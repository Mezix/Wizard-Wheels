using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapChunk : MonoBehaviour
{
    public Transform _offsetTransform;
    public Tilemap grassTileMap;
    public Tilemap stoneTilemap;
    public float[,] noiseMap;
    public bool _shown = true;

    public void Show(bool show)
    {
        _shown = show;
        grassTileMap.GetComponent<TilemapRenderer>().enabled = show;
        stoneTilemap.GetComponent<TilemapRenderer>().enabled = show;
    }
}
