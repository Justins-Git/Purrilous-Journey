using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EraManager : MonoBehaviour
{
    public static EraManager Instance;
    public Button evolveButton;
    public int currentEra = 0;
    public int currentXP = 0;

    public int[] xpThresholds = { 250, 500, 1500, 5000 }; // Cost to evolve
    public TMP_Text eraText;
    public TMP_Text xpText;
    public TMP_Text xpDisplayText; // Drag in from UI


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();

        evolveButton.onClick.AddListener(() => EraManager.Instance.TryEvolve());
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"Gained {amount} XP. Total: {currentXP}");
        UpdateUI();
    }

    public void TryEvolve()
    {
        if (currentEra >= xpThresholds.Length)
        {
            Debug.Log("🚫 Max era reached.");
            return;
        }

        int cost = xpThresholds[currentEra];
        if (currentXP >= cost)
        {
            currentXP -= cost;
            Evolve();
        }
        else
        {
            Debug.Log("❌ Not enough XP to evolve.");
        }
    }

    void Evolve()
    {
        currentEra++;
        Debug.Log($"🌟 Evolved to Era {currentEra + 1}!");
        PlayerBaseStats.Instance?.OnEraEvolve(currentEra);
        PlayerUnitSpawner.Instance?.OnEraEvolve(currentEra);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (eraText != null)
            eraText.text = $"Era: {currentEra + 1}";

        if (xpDisplayText != null)
        {
            string xpNeeded = currentEra < xpThresholds.Length
                ? xpThresholds[currentEra].ToString()
                : "-";

            xpDisplayText.text = $"XP: {currentXP} / {xpNeeded}";
        }
    }
}
