using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public int MaxHealth = 10;
    public int CurrentHealth { get; private set; }
    public int goldReward = 15; // This number is different per unit type in inspector
    public GameObject floatingGoldPrefab; // 🧾 Assign this in the prefab or runtime


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
            GoldManager.Instance?.AddGold(goldReward);

            if (floatingGoldPrefab != null)
            {
                GameObject text = Instantiate(floatingGoldPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                FloatingGold floating = text.GetComponent<FloatingGold>();
                if (floating != null)
                {
                    floating.SetText($"+{goldReward}");
                }
            }
        }


        Destroy(gameObject);
    }
}
