using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using GameSystem;
public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int height;
    public int width;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTile;
    [Header("MapObjects")]
    [SerializeField] private Transform groundObject;

    [Header("Ground Settings")]
    [SerializeField] private int dirtSpawnPercentage;
    [SerializeField] private int transitionSteps;
    [SerializeField] private int dirtCellThreshold;
    [SerializeField] private int stoneCellThreshold;
    [SerializeField] private int objectCount;
    [SerializeField] private TileDatas dirtTiles;
    [SerializeField] private MapObjectDatas MapObjects;
    private List<Vector3> objectPoses;

    public void GenerateMap()
    {
        //맵 타일 생성
        GenerateGround();

        //오브젝트 생성
        ObjectGenerate();
    }

    private void GenerateGround()
    {
        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int y = -height / 2; y < height / 2; y++)
            {
                groundTile.SetTile(new Vector3Int(x, y, 0), Random.Range(0, 100) < dirtSpawnPercentage ? dirtTiles.tiles[Random.Range(0, dirtTiles.tiles.Count)] : dirtTiles.tiles[Random.Range(0, dirtTiles.tiles.Count)]);
            }
        }
        SmoothGroundGeneration();
    }

    private void SmoothGroundGeneration()
    {
        for (int t = 0; t < transitionSteps; t++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    TileBase targetTile = groundTile.GetTile(new Vector3Int(x, y, 0));
                    List<TileBase> targetList = dirtTiles.tiles.Contains(targetTile) ? dirtTiles.tiles : dirtTiles.tiles;
                    int totalSimilarCells = CheckMoreCells(x, y, groundTile, targetList);
                    ReplaceTile(totalSimilarCells, x, y, targetTile, groundTile);
                }
            }
        }

        
    }


    private int CheckMoreCells(int x, int y, Tilemap tileMap, List<TileBase> targetList)
    {
        int totalSimilarTiles = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) { }
                else
                {
                    TileBase currentTile = tileMap.GetTile(new Vector3Int(x + i, y + j, 0));
                    if (targetList.Contains(currentTile) || currentTile == null) totalSimilarTiles++;
                }
            }
        }
        return totalSimilarTiles;
    }

    private void ReplaceTile(int totalSimilarTiles, int x, int y, TileBase targetTile, Tilemap tileMap)
    {
        if (dirtTiles.tiles.Contains(targetTile))
        {
            if (totalSimilarTiles < dirtCellThreshold)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), dirtTiles.tiles[Random.Range(0, dirtTiles.tiles.Count)]);
            }
        }
        else if (dirtTiles.tiles.Contains(targetTile))
        {
            if (totalSimilarTiles < stoneCellThreshold)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), dirtTiles.tiles[Random.Range(0, dirtTiles.tiles.Count)]);
            }
        }
    }

    public bool IsDirtTile(int x, int y)
    {
        return (dirtTiles.tiles.Contains(groundTile.GetTile(new Vector3Int(x, y, 0))));
    }


    private void ObjectGenerate()
    {
        SetObjectPos();
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = MapObjects.objects[Random.Range(0, MapObjects.objects.Count)];
            PoolManager.instance.Spawn(obj, objectPoses[i], Vector3.one, Quaternion.identity, true, groundObject);
        }
    }

    private void SetObjectPos()
    {
        objectPoses = new List<Vector3>();
        bool isSame = false;
        for (int i = 0; i < objectCount; i++)
        {
            while (isSame == false)
            {
                Vector3 pos = new Vector3();
                float x = Random.Range(-width / 2, width / 2);
                float y = Random.Range(-height / 2, height / 2);
                pos = new Vector2(x, y);

                for (int j = 0; j < objectPoses.Count; j++)
                {
                    if (objectPoses[j] == pos)
                    {
                        isSame = true;
                    }
                }

                if (isSame == false)
                {
                    objectPoses.Add(pos);
                }
            }
        }
    }

    
}
