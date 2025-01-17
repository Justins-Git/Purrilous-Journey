using System.Collections;
using UnityEngine;

public class UnitDamage : MonoBehaviour
{
    [SerializeField] private int damageToDeal = 2;
    [SerializeField] private float attackInterval = 10; // Time between attacks

    private UnitMovement parentMovement;
    private UnitHealth targetHealth;
    private bool isAttacking;

    private void Start()
    {
        // Grab the movement script from the parent
        parentMovement = GetComponentInParent<UnitMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if this collider belongs to an enemy unit or base
        // (You can adjust these tags as needed)
        if (other.CompareTag("EnemyUnit") || other.CompareTag("EnemyBase"))
        {
            // Stop moving
            if (parentMovement != null)
                parentMovement.canMove = false;

            // Get the enemy's health component
            targetHealth = other.GetComponent<UnitHealth>();

            if (targetHealth != null && !isAttacking)
            {
                // Start damaging the target repeatedly
                StartCoroutine(AttackTarget());
            }
        }
    }

    private IEnumerator AttackTarget()
    {
        isAttacking = true;
        // Keep attacking as long as the target is alive
        while (targetHealth != null && targetHealth.CurrentHealth > 0)
        {
            // Deal damage
            targetHealth.TakeDamage(damageToDeal);
            // Report enemy's current health
            Debug.Log(targetHealth);
            // Wait for the next attack cycle
            yield return new WaitForSeconds(attackInterval);
        }

        // If we get here, the target is null or dead
        ResumeMovement();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // If the enemy moves out of range before dying
        if (other.GetComponent<UnitHealth>() == targetHealth)
        {
            // Stop attacking
            StopAllCoroutines(); 
            ResumeMovement();
        }
    }

    private void ResumeMovement()
    {
        // Allow the unit to move again
        if (parentMovement != null)
            parentMovement.canMove = true;

        // Reset references
        targetHealth = null;
        isAttacking = false;
    }
}
