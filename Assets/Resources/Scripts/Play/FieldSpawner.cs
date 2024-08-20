using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSpawner : MonoBehaviour, ICustomUpdateMono
{
    public int spawnUnitCount;
    public float spawnTimer = 0f;
    private float spawnTime = 3f;
    private GameObject unitPrefab;

    void Start()
    {
        unitPrefab = Resources.Load<GameObject>("Prefabs/Enemy/Enemy");

        List<GameObject> prefabs = new List<GameObject>();
        List<Vector3> pos = new List<Vector3>();

        for(int i = 0; i < spawnUnitCount; i++)
        {
            prefabs.Add(unitPrefab);
        }
        pos = SpawnPositionSet(spawnUnitCount);
        UnitSpawn(prefabs, spawnUnitCount, pos);
    }
    void OnEnable()
    {
       CustomUpdateManager.customUpdateMonos.Add(this);
    }
    void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
    }

    public void CustomUpdate()
    {

    }

    protected void UnitSpawn(List<GameObject> prefab, int count, List<Vector3> spawnPos)
    {
        for(int i = 0; i < count; i++)
        {
            PoolManager.instance.Spawn(prefab[i], spawnPos[i], Vector3.one, Quaternion.identity, true, transform);    
        }
    }

    protected List<Vector3> SpawnPositionSet(int count)
    {
        List<Vector3> position = new List<Vector3>();
        HashSet<Vector3> existingPositions = new HashSet<Vector3>();

        // Ensure that the field has enough space to accommodate unique positions
        int maxAttempts = 10000;

        while (position.Count < count)
        {
            var (success, newPosition) = GenerateUniquePosition(existingPositions, maxAttempts);
            if (success)
            {
                position.Add(newPosition);
                existingPositions.Add(newPosition);
            }
            else
            {
                Debug.LogWarning("Unable to generate a unique position after 10,000 attempts.");
            }
        }

        return position;
    }

    private (bool, Vector3) GenerateUniquePosition(HashSet<Vector3> existingPositions, int maxAttempts)
    {
        int attempts = 0;
        Vector3 newPosition;

        do
        {
            float x = Random.Range(FieldActivity.instance.xMin, FieldActivity.instance.xMax);
            float y = Random.Range(FieldActivity.instance.yMin, FieldActivity.instance.yMax);
            newPosition = new Vector3(x, y, 0);

            attempts++;
        } while (existingPositions.Contains(newPosition) && attempts < maxAttempts);

        // Return a tuple containing a success flag and the new position
        bool success = attempts < maxAttempts;
        return (success, newPosition);
    }
}
