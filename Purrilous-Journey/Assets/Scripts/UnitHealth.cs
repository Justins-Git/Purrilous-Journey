using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public int MaxHealth = 10; // Maximum health of the unit
    public int CurrentHealth { get; private set; } // Public getter for health bar

    void Start()
    {
        CurrentHealth = MaxHealth; // Set starting health
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage! Remaining HP: {CurrentHealth}");

        // Ensure health doesn't drop below zero
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject); // Destroy the unit when health reaches zero
    }
}
