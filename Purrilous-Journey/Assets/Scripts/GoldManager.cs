using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [SerializeField] private PlayerBaseStats playerBaseStats; // 🟢 Assigned in Inspector
    public int currentGold = 0;
    public int startingGold = 50;

    public float passiveGoldInterval = 2f;
    public TMP_Text goldText; // 🟢 Assign in Inspector

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentGold = startingGold;
        UpdateGoldUI();
        InvokeRepeating(nameof(AddPassiveGold), passiveGoldInterval, passiveGoldInterval);
    }

    void AddPassiveGold()
    {
        if (playerBaseStats != null)
        {
            Transform baseTransform = playerBaseStats.GetCurrentBaseTransform();
            if (baseTransform != null)
            {
                BaseStats stats = baseTransform.GetComponent<BaseStats>();
                int goldToAdd = stats != null ? stats.goldPerTick : 5;
                AddGold(goldToAdd);
                Debug.Log($"🪙 Added {goldToAdd} gold from base.");
            }
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            return true;
        }

        Debug.Log("❌ Not enough gold!");
        return false;
    }

    void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {currentGold}";
    }
}
