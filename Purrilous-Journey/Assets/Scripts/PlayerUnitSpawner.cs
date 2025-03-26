using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerUnitSpawner : MonoBehaviour
{
    public GameObject[] unitPrefabs; // [0] = Melee, [1] = Archer, [2] = Tank
    public int[] unitCosts = new int[] { 25, 40, 60 }; // Costs: 0 = melee, 1 = archer, 2 = tank
    public Transform enemyBaseRef;
    public Transform spawnPoint;

    public float spawnCooldown = 1.5f; // 🔁 Global cooldown in seconds
    private bool canSpawn = true;

    public Button meleeButton;
    public Button archerButton;
    public Button tankButton;

    void Start()
    {
        meleeButton.onClick.RemoveAllListeners();
        archerButton.onClick.RemoveAllListeners();
        tankButton.onClick.RemoveAllListeners();

        meleeButton.onClick.AddListener(() => TrySpawnUnit(0));
        archerButton.onClick.AddListener(() => TrySpawnUnit(1));
        tankButton.onClick.AddListener(() => TrySpawnUnit(2));
    }

    void TrySpawnUnit(int index)
    {
        if (!canSpawn) return;

        int cost = unitCosts[index];

        if (!GoldManager.Instance.SpendGold(cost))
        {
            Debug.Log("Not enough gold to spawn this unit.");
            return;
        }

        StartCoroutine(SpawnWithDelay(index));
    }

    IEnumerator SpawnWithDelay(int index)
    {
        canSpawn = false;
        SpawnUnit(index);
        yield return new WaitForSeconds(spawnCooldown);
        canSpawn = true;
    }

    void SpawnUnit(int unitIndex)
    {
        if (unitIndex < 0 || unitIndex >= unitPrefabs.Length)
        {
            Debug.LogWarning("Invalid unit index!");
            return;
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject newUnit = Instantiate(unitPrefabs[unitIndex], spawnPos, Quaternion.identity);
        newUnit.layer = 6;

        UnitController uc = newUnit.GetComponent<UnitController>();
        if (uc != null)
        {
            uc.targetBase = enemyBaseRef;
            uc.enemyLayer = (1 << 7);
            uc.allyLayer = (1 << 6);
            uc.isPlayerUnit = true;
        }

        Debug.Log($"Spawned unit: {unitPrefabs[unitIndex].name}");
    }
}
