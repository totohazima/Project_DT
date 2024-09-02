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
    public List<Transform> fieldList = new List<Transform>();
    public List<FieldSpawner> fields = new List<FieldSpawner>();
    [HideInInspector] public float xMin, xMax, yMin, yMax;

    private float scanTimer;

    private void Awake()
    {
        instance = this;

        for(int i = 1; i < fieldList.Count; i++)
        {
            fields.Add(fieldList[i].GetComponent<FieldSpawner>());
        }

        LimitPosition();    
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
}
