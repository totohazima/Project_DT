using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "GameDataBase/TileData")]
public class TileDatas : ScriptableObject
{
    public List<TileBase> tiles;
}