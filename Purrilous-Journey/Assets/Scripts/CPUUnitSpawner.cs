using UnityEngine;

public class CPUUnitSpawner : MonoBehaviour
{
    [Header("Unit Prefabs")]
    public GameObject[] easyUnits;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform playerBaseRef;
    public float initialSpawnDelay = 2f;    
    public float initialSpawnInterval = 35f; 
    public float minSpawnInterval = 20f;   
    public float difficultyRampTime = 180f;  
    public float spawnIntervalDecreaseRate = 0.2f; 

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
            SpawnUnit();
            spawnTimer = 0f;
        }

        if (difficultyTimer >= difficultyRampTime)
        {
            RampDifficulty();
            difficultyTimer = 0f;
        }
    }

    private void SpawnUnit()
    {
        GameObject[] unitArray = easyUnits;
        int index = Random.Range(0, unitArray.Length);
        Vector3 spawnPos = spawnPoint ? spawnPoint.position : transform.position;
        
        GameObject newUnit = Instantiate(unitArray[index], spawnPos, Quaternion.identity);

        // ✅ Correctly sets layer using integer index
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

    private void RampDifficulty()
    {
        difficultyLevel++;
        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnIntervalDecreaseRate);
        Debug.Log($"CPU Difficulty Ramped to Level {difficultyLevel}. Spawn Interval: {currentSpawnInterval}");
    }
}
