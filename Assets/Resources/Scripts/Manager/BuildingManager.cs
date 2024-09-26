using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    public List<Building> buildings = new List<Building>();

    private void Awake()
    {
        Instance = this;

        for(int i = 0; i < transform.childCount; i++)
        {
            Building building = transform.GetChild(i).GetComponent<Building>();

            buildings.Add(building);
        }
    }
}
