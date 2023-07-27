using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus;
using System.Threading.Tasks;

public class MapGeneration : MonoBehaviour
{
    public List<TilemapChunk> _tilemapChunks = new List<TilemapChunk>();
    public NavMeshPlus.Components.NavMeshSurface navMeshSurface;

    [HideInInspector]
    public Grid grid;
    public Vector3 playerPosRelativeToGrid;
    public Vector3 camPos;
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
            UpdatePlayerPos();
        }
        TrackPlayerInMinimap();

        if (Input.GetKeyDown(KeyCode.U))
        {
            AddNewChunkAtPos(camPos);
        }
    }

    private void AddNewChunkAtPos(Vector3 camPos)
    {
        int xSign = Math.Sign(camPos.x);
        int ySign = Math.Sign(camPos.y);

        //  normalize
        camPos = new Vector2(Mathf.Abs(camPos.x), Mathf.Abs(camPos.y)) / chunkSize;

        // turn to int and reinsert the signs
        Vector2Int vectorToSpawnChunkAt = new Vector2Int(chunkSize / 2, chunkSize / 2) 
                                        + chunkSize * new Vector2Int(xSign * Mathf.CeilToInt(camPos.x), ySign * Mathf.CeilToInt(camPos.y));

        StartCoroutine(CreateNewTilemapChunk(vectorToSpawnChunkAt));
    }

    private void UpdatePlayerPos()
    {
        playerPosRelativeToGrid = grid.WorldToCell(REF.PlayerGO.transform.position);
        camPos = grid.WorldToCell(REF.Cam.transform.position);
    }

    private void GenerateInitialMap()
    {
        int initialradius = 1;
        for (int x = -initialradius; x < initialradius + 1; x++)
        {
            for (int y = -initialradius; y < initialradius + 1; y++)
            {
                StartCoroutine(CreateNewTilemapChunk(centeringVector + new Vector2Int(chunkSize * x, chunkSize * y)));
            }
        }
        CreateMinimap();
    }

    private IEnumerator CreateNewTilemapChunk(Vector2Int tilemapOffset)
    {
        TilemapChunk tilemapChunk = Instantiate(Resources.Load(GS.Prefabs("TilemapChunk"), typeof(TilemapChunk)) as TilemapChunk);
        tilemapChunk.transform.SetParent(transform, false);
        tilemapChunk.transform.localPosition = new Vector3(tilemapOffset.x / 2, tilemapOffset.y / 2, 0);
        tilemapChunk._offsetTransform.localPosition = -1 * new Vector2(chunkSize / (2f / grid.cellSize.x), chunkSize / (2f / grid.cellSize.y));
        _tilemapChunks.Add(tilemapChunk);
        tilemapChunk.noiseMap = new float[chunkSize, chunkSize];

        //  Setup Occlusion
        OcclusionCulling2D.ObjectSettings tilemapOcclusionSettings = new OcclusionCulling2D.ObjectSettings
        {
            _gameObjectToHide = tilemapChunk.gameObject,
            _tileMapChunk = tilemapChunk,
            size = new Vector2(chunkSize / (2f / grid.cellSize.x), chunkSize / (2f / grid.cellSize.y)),
            DrawColor = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f)),
            showBorders = false
        };
        tilemapOcclusionSettings.InitObjectSettingProperties();
        REF.Cam._occlusionCulling2D.objectSettings.Add(tilemapOcclusionSettings);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                tilemapChunk.noiseMap[x, y] = Mathf.PerlinNoise(perlinScale * (x + xPerlinOffset + tilemapOffset.x), perlinScale * (y + yPerlinOffset + tilemapOffset.y));

                if (tilemapChunk.noiseMap[x, y] < stoneThreshold) tilemapChunk.stoneTilemap.SetTile(new Vector3Int(x, y, 0), stoneRuleTile);
                if (tilemapChunk.noiseMap[x, y] < ornamentalThreshold)
                {
                    int index = UnityEngine.Random.Range(0, grassTiles.Length);
                    Tile grassTileToSet = (Tile)grassTiles[index];
                    float darkenedColor = 1 + (tilemapChunk.noiseMap[x, y] - ornamentalThreshold);
                    grassTileToSet.color = new Color(darkenedColor, darkenedColor, darkenedColor, 1);
                    tilemapChunk.grassTileMap.SetTile(new Vector3Int(x, y, 0), grassTileToSet);
                }
            }
        }
        yield return new WaitForEndOfFrame();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
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
