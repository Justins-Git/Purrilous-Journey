using UnityEngine;

public class CPUUnitSpawner : MonoBehaviour
{
    public static CPUUnitSpawner Instance;

    [Header("Era Unit Sets")]
    public GameObject[] era1Units; // [0]=melee, [1]=archer, [2]=tank
    public GameObject[] era2Units;
    public GameObject[] era3Units;
    public GameObject[] era4Units;
    public GameObject[] era5Units;

    [Header("Base Prefabs Per Era")]
    public GameObject[] basePrefabs; // one per era
    public Transform baseSpawnPoint;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform playerBaseRef;

    public float initialSpawnDelay = 2f;
    public float initialSpawnInterval = 35f;
    public float minSpawnInterval = 15f;
    public float spawnIntervalDecreaseRate = 1.5f;
    public float difficultyRampTime = 180f;
    public float eraAdvanceTime = 360f;

    private float currentSpawnInterval;
    private float spawnTimer;
    private float difficultyTimer;
    private float eraTimer;
    private int difficultyLevel;
    private int currentEra = 0;
    private GameObject currentBase;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        spawnTimer = initialSpawnDelay;
        difficultyTimer = 0f;
        eraTimer = 0f;

        EvolveEra(0); // start with Era 0 base + units
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;
        eraTimer += Time.deltaTime;

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

        if (eraTimer >= eraAdvanceTime && currentEra < 2)
        {
            EvolveEra(currentEra + 1);
            eraTimer = 0f;
        }
    }

    private void SpawnUnits()
    {
        int unitsToSpawn = 1;

        if (difficultyLevel >= 3 && Random.value < 0.25f)
            unitsToSpawn = 2;

        for (int i = 0; i < unitsToSpawn; i++)
            SpawnOneUnit();
    }

    private void SpawnOneUnit()
    {
        GameObject unitPrefab = ChooseUnit();
        if (unitPrefab == null) return;

        Vector3 spawnPos = spawnPoint ? spawnPoint.position : transform.position;
        GameObject newUnit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        newUnit.layer = 7;

        UnitController uc = newUnit.GetComponent<UnitController>();
        if (uc != null)
        {
            uc.targetBase = playerBaseRef;
            uc.enemyLayer = (1 << 6);
            uc.allyLayer = (1 << 7);
            uc.isPlayerUnit = false;
        }
    }

    private GameObject ChooseUnit()
    {
        GameObject[] currentSet = currentEra switch
        {
            0 => era1Units,
            1 => era2Units,
            2 => era3Units,
            _ => era1Units
        };

        if (difficultyLevel <= 1)
            return currentSet[0]; // melee

        if (difficultyLevel <= 3)
            return Random.value < 0.6f ? currentSet[0] : currentSet[1]; // melee + archer

        float r = Random.value;
        if (r < 0.5f) return currentSet[0];
        else if (r < 0.8f) return currentSet[1];
        else return currentSet[2];
    }

    private void RampDifficulty()
    {
        difficultyLevel++;
        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnIntervalDecreaseRate);
        Debug.Log($"CPU Difficulty Level {difficultyLevel}. Interval: {currentSpawnInterval}");
    }

    private void EvolveEra(int newEra)
    {
        currentEra = newEra;
        difficultyLevel = 0;
        currentSpawnInterval = initialSpawnInterval;

        if (currentBase != null)
            Destroy(currentBase);

        currentBase = Instantiate(basePrefabs[newEra], baseSpawnPoint.position, Quaternion.identity);

        PlayerUnitSpawner.Instance?.SetEnemyBase(currentBase.transform);
        Debug.Log($"🚀 CPU evolved to Era {newEra + 1}");
    }

    public Transform GetCurrentBaseTransform()
    {
        return currentBase?.transform;
    }

    public void UpdatePlayerBaseTarget(Transform newBase)
    {
        playerBaseRef = newBase;
        Debug.Log($"CPU now targeting new player base: {newBase.name}");

        // 🔁 Update all existing enemy units
        UnitController[] allUnits = FindObjectsOfType<UnitController>();
        foreach (UnitController uc in allUnits)
        {
            if (!uc.isPlayerUnit) // only affect CPU units
            {
                uc.targetBase = newBase;
            }
        }
    }
}
