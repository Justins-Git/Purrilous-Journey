using UnityEngine;

public class CPUUnitSpawner : MonoBehaviour
{
    [Header("Unit Prefabs")]
    public GameObject meleeCat;
    public GameObject archerCat;
    public GameObject tankCat;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform playerBaseRef;

    public float initialSpawnDelay = 2f;
    public float initialSpawnInterval = 35f;
    public float minSpawnInterval = 15f;
    public float difficultyRampTime = 180f;
    public float spawnIntervalDecreaseRate = 1.5f;

    private float currentSpawnInterval;
    private float spawnTimer;
    private float difficultyTimer;
    private int difficultyLevel;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        spawnTimer = initialSpawnDelay;
        difficultyTimer = 0f;
        difficultyLevel = 0;
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnUnits();
            spawnTimer = 0f;
        }

        if (difficultyTimer >= difficultyRampTime)
        {
            RampDifficulty();
            difficultyTimer = 0f;
        }
    }

    private void SpawnUnits()
    {
        int unitsToSpawn = 1;

        // Random chance to spawn 2 units on higher difficulties
        if (difficultyLevel >= 3 && Random.value < 0.25f)
        {
            unitsToSpawn = 2;
        }

        for (int i = 0; i < unitsToSpawn; i++)
        {
            SpawnOneUnit();
        }
    }

    private void SpawnOneUnit()
    {
        GameObject unitPrefab = ChooseUnitByDifficulty();
        if (unitPrefab == null) return;

        Vector3 spawnPos = spawnPoint ? spawnPoint.position : transform.position;
        GameObject newUnit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);

        newUnit.layer = 7;

        UnitController unitController = newUnit.GetComponent<UnitController>();
        if (unitController != null)
        {
            unitController.targetBase = playerBaseRef;
            unitController.enemyLayer = (1 << 6);
            unitController.allyLayer = (1 << 7);
            unitController.isPlayerUnit = false;
        }
    }

    private GameObject ChooseUnitByDifficulty()
    {
        // Melee only (Lv 0–1)
        if (difficultyLevel <= 1)
            return meleeCat;

        // Melee + Archers (Lv 2–3)
        if (difficultyLevel == 2 || difficultyLevel == 3)
            return Random.value < 0.6f ? meleeCat : archerCat;

        // All three (Lv 4+)
        float r = Random.value;
        if (r < 0.5f) return meleeCat;
        else if (r < 0.8f) return archerCat;
        else return tankCat;
    }

    private void RampDifficulty()
    {
        difficultyLevel++;
        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnIntervalDecreaseRate);
        Debug.Log($"CPU Difficulty Ramped to Level {difficultyLevel}. Spawn Interval: {currentSpawnInterval}");
    }
}
