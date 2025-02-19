using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitSpawner : MonoBehaviour
{
    public GameObject[] unitPrefabs;
    public Transform enemyBaseRef;
    public Transform spawnPoint; 
    public float spawnOffset = 2f;

    public void SpawnUnit(int unitIndex)
    {
        if (unitIndex < 0 || unitIndex >= unitPrefabs.Length) 
        {
            Debug.LogWarning("Invalid unit index!");
            return;
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject newUnit = Instantiate(unitPrefabs[unitIndex], spawnPoint.position, Quaternion.identity);

        // ✅ Correctly sets layer using integer index
        newUnit.layer = 6;

        UnitController unitController = newUnit.GetComponent<UnitController>();
        if (unitController != null)
        {
            unitController.targetBase = enemyBaseRef;
            unitController.enemyLayer = (1 << 7);
            unitController.allyLayer = (1 << 6);
            unitController.isPlayerUnit = true;
        }
    }
}
