using UnityEngine;
public class PlayerBaseStats : MonoBehaviour
{
    public static PlayerBaseStats Instance;

    public int baseHealth = 50;
    public int goldPerInterval = 1;

    void Awake()
    {
        Instance = this;
    }

    public void OnEraEvolve(int era)
    {
        baseHealth = (era^2)*50;
        goldPerInterval = era;

        Debug.Log($"Base stats boosted. HP: {baseHealth}, Gold Rate: {goldPerInterval}");
    }
}
