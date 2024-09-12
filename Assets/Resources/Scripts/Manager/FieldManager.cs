using GameSystem;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldManager : MonoBehaviour, ICustomUpdateMono
{
    public static FieldManager instance;
    public AstarPath astarPath;
    public CameraDrag cameraDrag;
    public Tilemap tilemap;
    public Transform spawnPool;

    public List<Transform> fieldList = new List<Transform>();
    public List<FieldActivity> fields = new List<FieldActivity>();
    public List<FieldSpawner> fieldSpawners = new List<FieldSpawner>();

    [HideInInspector] public float xMin, xMax, yMin, yMax;

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

        LimitPosition();    
    }

    void Start()
    {
        HeroSpawn();
    }
    void HeroSpawn()
    {
        GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/Player/Hero");
        GameObject hero = PoolManager.instance.Spawn(heroPrefab, Vector3.zero, Vector3.one, Quaternion.identity, true, spawnPool);
    }
    private void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);
    }
    private void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
    }

    public void CustomUpdate()
    {

    }

    private void LimitPosition()
    {
        BoundsInt bounds = tilemap.cellBounds;

        xMin = bounds.xMin;
        xMax = bounds.xMax;
        yMin = bounds.yMin;
        yMax = bounds.yMax;
    }

    public void AllFieldSpawn()
    {
        foreach(FieldSpawner field in fieldSpawners)
        {
            field.isSpawn = true;
        }
    }
}
