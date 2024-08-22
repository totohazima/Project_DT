using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldActivity : MonoBehaviour
{
    public static FieldActivity instance;
    public CameraDrag cameraDrag;
    public Tilemap tilemap;
    [HideInInspector] public float xMin, xMax, yMin, yMax;

    private void Awake()
    {
        instance = this;

        LimitPosition();    
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
