using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus;

public class MapGeneration : MonoBehaviour
{
    public List<TilemapChunk> _tilemapChunks = new List<TilemapChunk>();
    public NavMeshPlus.Components.NavMeshSurface navMeshSurface;

    [HideInInspector]
    public Grid grid;
    public Vector3 playerPosRelativeToGrid;
    private Vector2Int centeringVector;

    // Tiles
    private object[] grassTiles;
    private TileBase stoneRuleTile;

    //  Minimap Colors
    public Color minimapStoneColor;
    public Color minimapGrassColor;

    //  Generation

    public float perlinScale = 0.1f;
    public float stoneThreshold = 0.1f;
    public float ornamentalThreshold = 0.1f;
    float xPerlinOffset;
    float yPerlinOffset;

    public int chunkSize; //100 should be fine for screen width to not see the edges

    private Texture2D minimapTex;
    private Texture2D fogOfWarTex;
    private void Awake()
    {
        REF.MapGen = this;
        grid = GetComponentInChildren<Grid>();
        minimapTex = new Texture2D(chunkSize, chunkSize);
        fogOfWarTex = new Texture2D(chunkSize, chunkSize);
        grassTiles = Resources.LoadAll(GS.BGTiles("Grass Tiles"));
        stoneRuleTile = Resources.Load(GS.BGTiles("Stone Tiles/StoneRuleTile"), typeof(TileBase)) as TileBase;
        xPerlinOffset = UnityEngine.Random.Range(-10000f, 10000f);
        yPerlinOffset = UnityEngine.Random.Range(-10000f, 10000f);
        centeringVector = -1 * new Vector2Int(chunkSize / 2, chunkSize / 2);
    }
    private void Start()
    {
        navMeshSurface.BuildNavMesh();
        GenerateInitialMap();
    }
    private void Update()
    {
        if (REF.PlayerGO)
        {
            playerPosRelativeToGrid = grid.WorldToCell(REF.PlayerGO.transform.position);
        }
        //TrackPlayerInMinimap();
    }
    private void GenerateInitialMap()
    {
        for(int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                StartCoroutine(CreateNewTilemapChunk(centeringVector + new Vector2Int(chunkSize * x, chunkSize * y)));
            }
        }
        //Debug.Log("Mesh Generated");
        CreateMinimap();
    }

    private IEnumerator CreateNewTilemapChunk(Vector2Int pixelOffset)
    {
        TilemapChunk tilemapChunk = Instantiate(Resources.Load(GS.Prefabs("TilemapChunk"), typeof (TilemapChunk)) as TilemapChunk);
        tilemapChunk.transform.SetParent(transform, false);
        _tilemapChunks.Add(tilemapChunk);

        tilemapChunk.noiseMap = new float[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                tilemapChunk.noiseMap[x, y] = Mathf.PerlinNoise(perlinScale * (x + xPerlinOffset + pixelOffset.x), perlinScale * (y + yPerlinOffset + pixelOffset.y));
                if (tilemapChunk.noiseMap[x, y] < stoneThreshold) tilemapChunk.stoneTilemap.SetTile(new Vector3Int(x + pixelOffset.x, y + pixelOffset.y, 0), stoneRuleTile);
                if (tilemapChunk.noiseMap[x, y] < ornamentalThreshold)
                {
                    int index = UnityEngine.Random.Range(0, grassTiles.Length);
                    Tile grassTileToSet = (Tile) grassTiles[index];
                    float darkenedColor = 1 + (tilemapChunk.noiseMap[x, y] - ornamentalThreshold);
                    grassTileToSet.color = new Color(darkenedColor, darkenedColor, darkenedColor, 1);
                    tilemapChunk.grassTileMap.SetTile(new Vector3Int(x + pixelOffset.x, y + pixelOffset.y, 0), grassTileToSet);
                }
            }
        }
        yield return new WaitForEndOfFrame();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        Debug.Log("done");
        
    }

    //  Minimap

    private void CreateMinimap()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                //Tile tile = tilemap.GetTile(new Vector3Int(x, y, 0)) as Tile;
                //Sprite sprite = tile.sprite;
                //Color color = AverageColorFromTexture(sprite);
                //minimapTex.SetPixel(x, y, color);

                if (_tilemapChunks[0].noiseMap[x, y] < stoneThreshold) minimapTex.SetPixel(x, y, minimapStoneColor);
                else minimapTex.SetPixel(x, y, minimapGrassColor);
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
        REF.UI._minimapScript._bigMapRect.sizeDelta = new Vector2(chunkSize, chunkSize);
        REF.UI._minimapScript._bigMapImage.sprite = minimapSprite;

        REF.UI._minimapScript._fogOfWarRect.sizeDelta = new Vector2(chunkSize, chunkSize);
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
}
