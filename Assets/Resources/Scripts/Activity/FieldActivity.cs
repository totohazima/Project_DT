using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldActivity : MonoBehaviour, ICustomUpdateMono
{
    public static FieldActivity instance;
    public AstarPath astarPath;
    public CameraDrag cameraDrag;
    public Tilemap tilemap;
    [HideInInspector] public float xMin, xMax, yMin, yMax;

    private float scanTimer;

    private void Awake()
    {
        instance = this;

        WalkableScan();
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
        scanTimer += Time.deltaTime;
        if(scanTimer > 10f)
        {
            WalkableScan();
        }
    }

    protected void WalkableScan()
    {
        astarPath.Scan();

        scanTimer = 0f;
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
