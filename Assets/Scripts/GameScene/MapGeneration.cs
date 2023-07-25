﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour
{
    public Tilemap grassTileMap;
    public Tilemap stoneTilemap;
    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public Vector3 playerPosRelativeToGrid;

    public TileBase _ruleTile;

    private List<Tile> spawnableTiles = new List<Tile>();

    //  Minimap Colors
    public Color StoneColor;
    public Color GrassColor;

    //  Generation

    public int[,] map;

    public int chunkWidth;
    public int chunkHeight;
    [Range(0, 100)]
    public int RandomFillPercent;

    public string seed;
    public bool useRandomSeed;

    private Texture2D minimapTex;
    private Texture2D fogOfWarTex;
    private void Awake()
    {
        grid = GetComponentInChildren<Grid>();
        REF.MapGen = this;

        InitTiles();
    }
    private void Start()
    {
        minimapTex = new Texture2D(chunkWidth, chunkHeight);
        fogOfWarTex = new Texture2D(chunkWidth, chunkHeight);
        GenerateMap();
    }
    private void Update()
    {
        if (REF.PlayerGO)
        {
            playerPosRelativeToGrid = grid.WorldToCell(REF.PlayerGO.transform.position);
        }
        TrackPlayerInMinimap();
    }
    private void InitTiles()
    {
        object[] tiles = Resources.LoadAll(GS.Tiles("BG Tiles"), typeof (Tile));
        foreach (object t in tiles)
        {
            Tile tile = (Tile)t;
            spawnableTiles.Add(tile);
        }
    }
    private void GenerateMap()
    {
        map = new int[chunkWidth, chunkHeight];
        RandomFillMap();

        int iterations = 5;

        for (int i = 0; i < iterations; i++)
        {
            SmoothMap();
        }
        Vector2Int centeringVector = -1 * new Vector2Int(chunkWidth / 2, chunkHeight / 2);
        SpawnTilemap(centeringVector);
        CreateMinimap();
    }
    private void RandomFillMap()
    {
        if (useRandomSeed) seed = Time.time.ToString();

        System.Random pseudoRandomnumber = new System.Random(seed.GetHashCode());
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkWidth; y++)
            {
                if (x == 0 || x == chunkWidth - 1 || y == 0 || y == chunkHeight - 1) map[x, y] = 1;
                else
                {
                    map[x, y] = (pseudoRandomnumber.Next(0, 100) < RandomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    private void SpawnTilemap(Vector2Int offset)
    {
        object[] grassTiles = Resources.LoadAll(GS.BGTiles("Grass Tiles"));
        TileBase stoneRuleTile = Resources.Load(GS.BGTiles("Stone Tiles/StoneRuleTile"), typeof(TileBase)) as TileBase;

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkWidth; y++)
            {
                int index = UnityEngine.Random.Range(0, grassTiles.Length);
                Tile grassTileToSet = (Tile) grassTiles[index];

                grassTileMap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), grassTileToSet);

                if (map[x, y] == 1)
                {
                    stoneTilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), stoneRuleTile);
                    grassTileMap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), grassTileToSet);
                }

            }
        }
    }

    private void SmoothMap()
    {
        int threshold = 4;
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkWidth; y++)
            {
                int neighbourWallTiles = GetSurroundingWallsCount(x, y);

                if (neighbourWallTiles > threshold)
                {
                    map[x, y] = 1;
                }
                else if (neighbourWallTiles < threshold)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    private int GetSurroundingWallsCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < chunkWidth && neighbourY >= 0 && neighbourY < chunkHeight)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    //  Minimap

    private void CreateMinimap()
    {
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                //Tile tile = tilemap.GetTile(new Vector3Int(x, y, 0)) as Tile;
                //Sprite sprite = tile.sprite;
                //Color color = AverageColorFromTexture(sprite);
                //minimapTex.SetPixel(x, y, color);

                if (map[x, y] == 1) minimapTex.SetPixel(x, y, StoneColor);
                else minimapTex.SetPixel(x, y, GrassColor);
                fogOfWarTex.SetPixel(x, y, Color.grey);
            }
        }
        minimapTex.filterMode = FilterMode.Point;
        minimapTex.Apply();

        fogOfWarTex.filterMode = FilterMode.Point;
        fogOfWarTex.Apply();

        Sprite minimapSprite = Sprite.Create(minimapTex,
                                             new Rect(0.0f, 0.0f, minimapTex.width, minimapTex.height),
                                             new Vector2(0.5f, 0.5f),
                                             100.0f);
        REF.UI._minimapScript._bigMapRect.sizeDelta = new Vector2(chunkWidth, chunkHeight);
        REF.UI._minimapScript._bigMapImage.sprite = minimapSprite;

        REF.UI._minimapScript._fogOfWarRect.sizeDelta = new Vector2(chunkWidth, chunkHeight);
        REF.UI._minimapScript._fogOfWarImage.sprite = Sprite.Create(fogOfWarTex,
                                             new Rect(0.0f, 0.0f, fogOfWarTex.width, fogOfWarTex.height),
                                             new Vector2(0.5f, 0.5f),
                                             100.0f);
    }
    private void TrackPlayerInMinimap()
    {
        float scale = 2;
        REF.UI._minimapScript._bigMapRect.anchoredPosition = -1 * REF.PCon.transform.position * scale;
        UpdateFogOfWar(-1 * REF.UI._minimapScript._bigMapRect.anchoredPosition);
    }
    private void UpdateFogOfWar(Vector2 playerPos)
    {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(playerPos.x), Mathf.RoundToInt(playerPos.y));

        float ratio = Screen.width / (float)Screen.height;
        int visionRadiusWidth = Mathf.FloorToInt(20 * ratio);
        int visionRadiusHeight = Mathf.FloorToInt(20 / ratio);
        int radius = Math.Max(visionRadiusWidth, visionRadiusHeight);

        DrawCircle(fogOfWarTex, pos.x, pos.y, radius, Color.clear);
        REF.UI._minimapScript._fogOfWarImage.sprite = Sprite.Create(fogOfWarTex,
                                             new Rect(0.0f, 0.0f, fogOfWarTex.width, fogOfWarTex.height),
                                             new Vector2(0.5f, 0.5f),
                                             100.0f);
    }
    private void DrawCircle(Texture2D tex, int x, int y, int radius, Color color)
    {
        float rSquared = radius * radius;
        x += tex.width / 2;
        y += tex.height / 2;

        for (int u = x - radius; u < x + radius + 1; u++)
            for (int v = y - radius; v < y + radius + 1; v++)
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                    tex.SetPixel(u, v, color);

        tex.Apply();
    }
    private Color AverageColorFromTexture(Sprite sprite)
    {
        Texture2D tex = sprite.texture;
        print(tex);
        Color32[] texColors = tex.GetPixels32();
        int total = texColors.Length;
        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < total; i++)
        {

            r += texColors[i].r;
            g += texColors[i].g;
            b += texColors[i].b;

        }
        return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 255);
    }

    //  Old
    /*
    private void CreateTilemap(int radiusX, int radiusY, Vector2 vec)
    {
        int startPosX = Mathf.FloorToInt(vec.x);
        int startPosY = Mathf.FloorToInt(vec.y);

        for (int x = -radiusX; x < radiusX; x++)
        {
            for (int y = -radiusY; y < radiusY; y++)
            {
                if (grassTileMap.GetTile(new Vector3Int(x + startPosX, y + startPosY, 0)) == null)
                {
                    grassTileMap.SetTile(new Vector3Int(x + startPosX, y + startPosY, 0), spawnableTiles[UnityEngine.Random.Range(0, spawnableTiles.Count - 1)]);
                }
            }
        }
    }*/
}
