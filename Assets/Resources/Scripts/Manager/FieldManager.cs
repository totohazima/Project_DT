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
    public CameraController_InGame cameraController;
    public Transform spawnPool;
    public bool isAlreadyBossSpawn = false;
    public List<Transform> fieldList = new List<Transform>();
    [HideInInspector] public List<FieldActivity> fields = new List<FieldActivity>();
    [HideInInspector] public List<FieldSpawner> fieldSpawners = new List<FieldSpawner>();

    [Range(0, 10)] public int spawnHeroCount = 1;
    public List<HeroCharacter> heroList = new List<HeroCharacter>();

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
        AllFieldSpawn();
    }

    private void HeroSpawn()
    {
        List<Vector3> pos = new List<Vector3>();
        pos = SpawnPointSet(spawnHeroCount);

        for (int i = 0; i < pos.Count; i++)
        {
            GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/Player/Hero");

            HeroCharacter character = heroPrefab.GetComponent<HeroCharacter>();
            CharacterInfoTable.Data data = GameManager.instance.gameDataBase.characterInfoTable.table[i];

            character.code = data.characterCode;
            character.characterName = data.characterName;
            character.jobClass = data.characterClass;
            character.characterCostume.dressCostume_Code = data.characterCostumeCode;

            GameObject hero = PoolManager.instance.Spawn(character.gameObject, pos[i], Vector3.one, Quaternion.identity, true, spawnPool);
            TestAlgorithm algorithm = hero.GetComponent<TestAlgorithm>();
            if (i <= 4)
            {
                algorithm.targetField = FieldMap.Field.DESERT;
            }
            else
            {
                algorithm.targetField = FieldMap.Field.SNOW;
            }

            HeroCharacter heroCharacter = hero.GetComponent<HeroCharacter>();
            heroList.Add(heroCharacter);
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
