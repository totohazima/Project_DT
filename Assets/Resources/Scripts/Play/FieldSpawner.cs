using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldHelper;
public class FieldSpawner : MonoBehaviour, ICustomUpdateMono
{
    public Transform getTransform;
    public FieldMap.Field controlField;
    public bool isSpawn = false; //true일 경우 리스폰
    public int spawnUnitCount;
    private GameObject unitPrefab;
    

    void Start()
    {
        unitPrefab = Resources.Load<GameObject>("Prefabs/Enemy/Enemy");
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
        if (isSpawn)
        {
            SpawnSetting();
            isSpawn = false;
        }
    }

    protected void SpawnSetting()
    {
        List<GameObject> prefabs = new List<GameObject>();
        List<Vector3> pos = new List<Vector3>();

        for (int i = 0; i < spawnUnitCount; i++)
        {
            prefabs.Add(unitPrefab);
        }
        pos = SpawnPositionSet(spawnUnitCount);
        UnitSpawn(prefabs, spawnUnitCount, pos);
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
        Vector3 newPosition = Vector3.zero;

        //Vector3 originPostion = fieldBoundaryCollider.transform.position;
        do
        {
            //float range_X = fieldBoundaryCollider.bounds.size.x;
            //float range_Y = fieldBoundaryCollider.bounds.size.y;

            //range_X = Random.Range((range_X / 2) * -1, range_X / 2);
            //range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
            //newPosition = new Vector3(range_X, range_Y, 0);
            //newPosition = newPosition + originPostion;

            attempts++;
        } 
        while 
        (
            existingPositions.Contains(newPosition) && attempts < maxAttempts
        );

        // Return a tuple containing a success flag and the new position
        bool success = attempts < maxAttempts;
        return (success, newPosition);
    }
}
