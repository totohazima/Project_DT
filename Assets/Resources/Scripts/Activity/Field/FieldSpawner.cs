using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSpawner : MonoBehaviour, ICustomUpdateMono
{
    public FieldActivity fieldActivity;
    public bool isSpawn = false; //true일 경우 리스폰
    public int spawnUnitCount;
    public Transform spawnPointGroup;
    public Transform spawnPools;
    public List<Transform> spawnPoints = new List<Transform>();

    private GameObject unitPrefab;
    private EnemyCharacter monster;

    void Awake()
    {
        for(int i = 0; i < spawnPointGroup.childCount; i++)
        {
            spawnPoints.Add(spawnPointGroup.GetChild(i).transform);
        }

        unitPrefab = Resources.Load<GameObject>("Prefabs/Enemys/mn_000");
        monster = unitPrefab.GetComponent<EnemyCharacter>();
    }
    private void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);   
    }
    public void CustomUpdate()
    {
        if(isSpawn)
        {
            SpawnSetting();
            isSpawn = false;
        }
    }
    private void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
    }

    protected void SpawnSetting()
    {
        List<EnemyCharacter> prefabs = new List<EnemyCharacter>();
        List<Vector3> pos = new List<Vector3>();

        for (int i = 0; i < spawnUnitCount; i++)
        {
            prefabs.Add(monster);
        }
        pos = SpawnPointSet(spawnUnitCount);
        UnitSpawn(prefabs, spawnUnitCount, pos);
    }
    protected void UnitSpawn(List<EnemyCharacter> prefab, int count, List<Vector3> spawnPos)
    {
        for (int i = 0; i < count; i++)
        {
            prefab[i].myField = fieldActivity.controlField;
            GameObject monster = PoolManager.instance.Spawn(prefab[i].gameObject, spawnPos[i], Vector3.one, Quaternion.identity, true, spawnPools);
            monster.transform.position = spawnPos[i];
            fieldActivity.monsters.Add(prefab[i]);
        }
    }

    protected List<Vector3> SpawnPointSet(int count)
    {
        List<Vector3> pos = new List<Vector3>();

        for(int i = 0; i < count; i++)
        {
            int index = Random.Range(0, spawnPoints.Count);
            Vector3 randPos = spawnPoints[index].position;
            pos.Add(randPos);
        }

        return pos;
    }

}
