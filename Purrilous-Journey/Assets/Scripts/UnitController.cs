using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float detectionRadius = 5f;
    public float attackRange = 1.8f;
    public float baseAttackRange = 2.5f;
    public float queueDistance = 1.0f;
    public float maxQueueCheckDistance = 3.5f;
    public int attackDamage = 2;
    public float attackCooldown = 1.5f;
    public float attackStartDelay = 0.25f;

    public LayerMask enemyLayer;
    public LayerMask allyLayer;
    public Transform targetBase;
    public bool isPlayerUnit;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController playerAnimator;
    public RuntimeAnimatorController enemyAnimator;

    private Transform currentTarget;
    private Transform frontAlly;
    private bool isMoving = true;
    private bool isAttacking = false;
    private bool isWaitingInQueue = false;
    private float lastAttackTime;

    private Animator animator;

    void Start()
    {
        AcquireTarget();
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            // Assign the correct Animator Controller at runtime
            animator.runtimeAnimatorController = isPlayerUnit ? playerAnimator : enemyAnimator;
        }
    }

    void Update()
    {
        FindFrontAlly();
        AcquireTarget();

        if (currentTarget == null)
        {
            currentTarget = targetBase;
        }

        float attackDistance = currentTarget == targetBase ? baseAttackRange : attackRange;

        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.position) <= attackDistance)
        {
            isMoving = false;
            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
            {
                StartCoroutine(Attack());
            }
        }
        else if (frontAlly != null)
        {
            float currentDistanceToAlly = Mathf.Abs(transform.position.x - frontAlly.position.x);
            if (currentDistanceToAlly > queueDistance * 0.8f)
            {
                isMoving = true;
                isWaitingInQueue = false;
            }
            else
            {
                isMoving = false;
                isWaitingInQueue = true;
            }
        }
        else
        {
            isMoving = true;
            isWaitingInQueue = false;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }

        if (isMoving)
        {
            Move();
        }
    }

    void FindFrontAlly()
    {
        Collider2D[] alliesInRange = Physics2D.OverlapCircleAll(transform.position, maxQueueCheckDistance, allyLayer);
        frontAlly = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D ally in alliesInRange)
        {
            float distanceToAlly = Vector2.Distance(transform.position, ally.transform.position);
            if (distanceToAlly < closestDistance && ally.transform != transform &&
                ((isPlayerUnit && ally.transform.position.x > transform.position.x) ||
                (!isPlayerUnit && ally.transform.position.x < transform.position.x)))
            {
                frontAlly = ally.transform;
                closestDistance = distanceToAlly;
            }
        }
    }

    Transform GetClosestEnemy(Collider2D[] enemies)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }

        return closest;
    }

    void AcquireTarget()
    {
        if (frontAlly != null)
        {
            UnitController allyController = frontAlly.GetComponent<UnitController>();
            if (allyController != null && allyController.currentTarget != targetBase)
            {
                currentTarget = allyController.currentTarget;
                return;
            }
        }

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        if (enemiesInRange.Length > 0)
        {
            currentTarget = GetClosestEnemy(enemiesInRange);
        }
        else
        {
            currentTarget = targetBase;
        }
    }

    void Move()
    {
        if (currentTarget != null && !isWaitingInQueue)
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
            if (distanceToTarget > attackRange)
            {
                Vector2 direction = (currentTarget.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackStartDelay);

        if (currentTarget != null)
        {
            float attackDistance = currentTarget == targetBase ? baseAttackRange : attackRange;
            if (Vector2.Distance(transform.position, currentTarget.position) <= attackDistance)
            {
                if (currentTarget.CompareTag("EnemyBase") || currentTarget.CompareTag("PlayerBase") || currentTarget == targetBase)
                {
                    UnitHealth baseHealth = currentTarget.GetComponent<UnitHealth>();
                    if (baseHealth != null)
                    {
                        baseHealth.TakeDamage(attackDamage);
                        Debug.Log($"{gameObject.name} dealt {attackDamage} damage to the enemy base!");
                    }
                }
                else
                {
                    UnitHealth targetHealth = currentTarget.GetComponent<UnitHealth>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(attackDamage);
                        Debug.Log($"{gameObject.name} attacks {currentTarget.name} for {attackDamage} damage!");

                        if (targetHealth.CurrentHealth <= 0)
                        {
                            currentTarget = null;
                            isMoving = true;
                        }
                    }
                }
            }
        }

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}
