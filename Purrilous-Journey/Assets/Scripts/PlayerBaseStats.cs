using UnityEngine;

public class PlayerBaseStats : MonoBehaviour
{
    public static PlayerBaseStats Instance;

    [SerializeField] private Transform baseParent;
    public GameObject[] baseEraPrefabs;
    private GameObject currentBaseInstance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        OnEraEvolve(0);
    }

    public void OnEraEvolve(int era)
    {
        Debug.Log($"[PlayerBaseStats] Evolving to era {era}");

        Debug.Log($"{baseEraPrefabs.Length} is hopefully not 0");
        if (era < baseEraPrefabs.Length)
        {
            // Move or rename the old base before destroying
            if (currentBaseInstance != null)
            {
                Debug.Log($"Destroying old base: {currentBaseInstance.name}");
                DestroyImmediate(currentBaseInstance); // 💥 Use DestroyImmediate to force it now
            }

            GameObject newBase = Instantiate(baseEraPrefabs[era], baseParent.position, Quaternion.identity, baseParent);
            currentBaseInstance = newBase;

            CPUUnitSpawner.Instance?.UpdatePlayerBaseTarget(currentBaseInstance.transform);
            PlayerUnitSpawner.Instance?.SetEnemyBase(CPUUnitSpawner.Instance?.GetCurrentBaseTransform());
            

            BaseStats stats = currentBaseInstance.GetComponent<BaseStats>();
            if (stats != null)
            {
                Debug.Log($"Spawned new base: {newBase.name} | HP: {stats.maxHealth}, Gold/tick: {stats.goldPerTick}");
            }
            else
            {
                Debug.LogWarning("New base spawned but no BaseStats found!");
            }

            PlayerUnitSpawner.Instance?.SetEnemyBase(currentBaseInstance.transform);
            CPUUnitSpawner.Instance?.UpdatePlayerBaseTarget(currentBaseInstance.transform);
        }
        else
        {
            Debug.LogError($"Invalid era index: {era}");
        }
    }

    public Transform GetCurrentBaseTransform()
    {
        return currentBaseInstance?.transform;
    }
}
