using FieldHelper;
using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldManager : MonoBehaviour
{
    public static FieldManager instance;
    public AstarPath astarPath;
    public CameraDrag cameraDrag;
    public Transform spawnPool;

    public List<Transform> fieldList = new List<Transform>();
    [HideInInspector] public List<FieldActivity> fields = new List<FieldActivity>();
    [HideInInspector] public List<FieldSpawner> fieldSpawners = new List<FieldSpawner>();

    [Range(0, 10)] public int spawnHeroCount = 1;

    private void Awake()
    {
        instance = this;

        for(int i = 0; i < fieldList.Count; i++)
        {
            if (fieldList[i].GetComponent<FieldActivity>() != null)
            {
                fields.Add(fieldList[i].GetComponent<FieldActivity>());
            }
            if (fieldList[i].GetComponentInChildren<FieldSpawner>() != null)
            {
                fieldSpawners.Add(fieldList[i].GetComponentInChildren<FieldSpawner>());
            }
        }   
    }

    void Start()
    {
        HeroSpawn();
    }
    private void HeroSpawn()
    {
        List<Vector3> pos = new List<Vector3>();
        pos = SpawnPointSet(spawnHeroCount);

        for (int i = 0; i < pos.Count; i++)
        {
            GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/Player/Hero");

            Character character = heroPrefab.GetComponent<Character>();
            CharacterInfoTable.Data data = GameManager.instance.gameDataBase.characterInfoTable.table[i];

            character.code = data.characterCode;
            character.name = data.characterName;
            character.jobClass = data.characterClass;
            character.characterCostume.dressCostume_Code = data.characterCostumeCode;

            GameObject hero = PoolManager.instance.Spawn(character.gameObject, pos[i], Vector3.one, Quaternion.identity, true, spawnPool);
            TestCharacterAlgorithm_Step3 algorithm = hero.GetComponent<TestCharacterAlgorithm_Step3>();
            if (i <= 4)
            {
                algorithm.combatField = FieldMap.Field.DESERT;
            }
            else
            {
                algorithm.combatField = FieldMap.Field.SNOW;
            }
        }
    }
    protected List<Vector3> SpawnPointSet(int count)
    {
        List<Vector3> pos = new List<Vector3>();
        FieldSpawner spawner = fieldSpawners[(int)FieldMap.Field.VILLAGE];

        float[] chanceList = new float[spawner.spawnPoints.Count];

        for (int i = 0; i < chanceList.Length; i++)
        {
            chanceList[i] = 1f / chanceList.Length; // 각 스폰 포인트의 확률을 동일하게 설정
        }

        for (int i = 0; i < count; i++)
        {
            int index = GameManager.instance.Judgment(chanceList);

            Vector3 randPos = spawner.spawnPoints[index].position;
            pos.Add(randPos);
        }

        return pos;
    }


    public void AllFieldSpawn()
    {
        foreach(FieldSpawner field in fieldSpawners)
        {
            field.isSpawn = true;
        }
    }
}
