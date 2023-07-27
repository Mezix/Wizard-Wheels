using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapChunk : MonoBehaviour
{
    public Tilemap grassTileMap;
    public Tilemap stoneTilemap;
    public float[,] noiseMap;

    public void Show(bool show)
    {
        grassTileMap.GetComponent<TilemapRenderer>().enabled = show;
        stoneTilemap.GetComponent<TilemapRenderer>().enabled = show;
    }
}
