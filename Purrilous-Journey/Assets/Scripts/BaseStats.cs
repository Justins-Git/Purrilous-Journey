using UnityEngine;

public class BaseStats : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;

    public int goldPerTick = 1;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Base took {amount} damage! Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("🏚 Base has been destroyed!");
            // TODO add lose/win logic here
        }
    }

    public void OnEraEvolve(int newEra)
    {
        maxHealth = 50*(newEra^2);
        goldPerTick = newEra;

        currentHealth = maxHealth;
        Debug.Log($"Base stats upgraded for Era {newEra + 1}: HP {maxHealth}, GoldTick {goldPerTick}");
    }
}
