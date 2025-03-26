using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public int MaxHealth = 10;
    public int CurrentHealth { get; private set; }

    private UnitController unitController;

    void Start()
    {
        CurrentHealth = MaxHealth;
        unitController = GetComponent<UnitController>(); 
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage! Remaining HP: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        // Only reward gold if this was an enemy unit
        if (unitController != null && !unitController.isPlayerUnit)
        {
            GoldManager.Instance?.AddGold(10);
        }

        Destroy(gameObject);
    }
}
