using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float detectionRadius = 5f;
    public float attackRange = 1.8f; // ✅ Slightly increased to ensure base detection
    public float baseAttackRange = 2.5f; // ✅ Larger range for attacking bases
    public float queueDistance = 1.0f; // ✅ Reduced for tighter queue spacing
    public float maxQueueCheckDistance = 3.5f;
    public int attackDamage = 2;
    public float attackCooldown = 1.5f;
    public float attackStartDelay = 0.25f;
    public LayerMask enemyLayer;
    public LayerMask allyLayer;
    public Transform targetBase;
    public bool isPlayerUnit;

    private Transform currentTarget;
    private Transform frontAlly;
    private bool isMoving = true;
    private bool isAttacking = false;
    private bool isWaitingInQueue = false;
    private float lastAttackTime;

    void Start()
    {
        AcquireTarget();
    }

    void Update()
    {
        FindFrontAlly();
        AcquireTarget();

        if (currentTarget == null)
        {
            Debug.LogWarning($"{gameObject.name} has no target. Moving to base.");
            currentTarget = targetBase;
        }
        else
        {
            Debug.Log($"{gameObject.name} is targeting {currentTarget.name}");
        }

        // ✅ Stop moving if an enemy or base is in attack range
        float attackDistance = currentTarget == targetBase ? baseAttackRange : attackRange;
        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.position) <= attackDistance)
        {
            isMoving = false;
            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
            {
                StartCoroutine(Attack());
            }
        }
        // ✅ MOVE UNTIL UNIT IS RIGHT BEHIND ITS ALLY
        else if (frontAlly != null)
        {
            float stoppingPosition = isPlayerUnit 
                ? frontAlly.position.x - queueDistance
                : frontAlly.position.x + queueDistance;

            float currentDistanceToAlly = Mathf.Abs(transform.position.x - frontAlly.position.x);

            if (currentDistanceToAlly > queueDistance * 0.8f) // ✅ Keep moving until very close
            {
                isMoving = true;
                isWaitingInQueue = false;
                Debug.Log($"{gameObject.name} is MOVING CLOSER to {frontAlly.name} (current gap: {currentDistanceToAlly}).");
            }
            else
            {
                isMoving = false;
                isWaitingInQueue = true;
                Debug.Log($"{gameObject.name} is STOPPING behind {frontAlly.name} at {stoppingPosition}.");
            }
        }
        else
        {
            isMoving = true;
            isWaitingInQueue = false;
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
            
            if (distanceToAlly < closestDistance && distanceToAlly <= maxQueueCheckDistance &&
                ally.transform != transform && 
                ((isPlayerUnit && ally.transform.position.x > transform.position.x) || 
                (!isPlayerUnit && ally.transform.position.x < transform.position.x)))
            {
                frontAlly = ally.transform;
                closestDistance = distanceToAlly;
            }
        }

        if (frontAlly != null)
        {
            Debug.Log($"{gameObject.name} detected {frontAlly.name} in front at distance {closestDistance}.");
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
                Debug.Log($"{gameObject.name} is now targeting {currentTarget.name} because {frontAlly.name} is fighting it.");
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
            Debug.Log($"{gameObject.name} is setting target to BASE.");
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
                Debug.Log($"{gameObject.name} is moving towards {currentTarget.name}");
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackStartDelay);

        if (currentTarget != null)
        {
            float attackDistance = currentTarget == targetBase ? baseAttackRange : attackRange;
            if (Vector2.Distance(transform.position, currentTarget.position) <= attackDistance)
            {
                if (currentTarget.CompareTag("Base") || currentTarget == targetBase)
                {
                    Debug.Log($"{gameObject.name} is attacking the enemy BASE!");
                    UnitHealth baseHealth = currentTarget.GetComponent<UnitHealth>();
                    if (baseHealth != null)
                    {
                        baseHealth.TakeDamage(attackDamage);
                        Debug.Log($"{gameObject.name} dealt {attackDamage} damage to the enemy base!");
                    }
                    else
                    {
                        Debug.LogError($"Base {currentTarget.name} does not have a UnitHealth component!");
                    }
                }
                else
                {
                    UnitHealth targetHealth = currentTarget.GetComponent<UnitHealth>();
                    if (targetHealth != null)
                    {
                        Debug.Log($"{gameObject.name} attacks {currentTarget.name} for {attackDamage} damage!");
                        targetHealth.TakeDamage(attackDamage);

                        if (targetHealth.CurrentHealth <= 0)
                        {
                            Debug.Log($"{currentTarget.name} has died!");
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
