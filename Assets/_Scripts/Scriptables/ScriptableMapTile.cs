using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Map Tile", menuName = "Map Tile")]
public class ScriptableMapTile : ScriptableObject
{
    [Header("Tile")]
    public List<TileBase> Tiles;

    [Header("Walkable")]
    public bool IsWalkable = true;
    public float WalkSpeedModifier = 1f;

    //[Header("Build")]
    // to come
}
