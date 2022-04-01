using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static TankRoomConstellation playerTankConstellationFromSelectScreen;

    private Tilemap tilemap;
    private Grid grid;
    private Vector3 playerPosRelativeToGrid;

    private List<Tile> spawnableTiles = new List<Tile>();

    //  Minimap Colors
    public Color StoneColor;
    public Color GrassColor;

    //  Generation

    public int[,] map;

    public int chunkWidth;
    public int chunkHeight;
    [Range(0,100)]
    public int RandomFillPercent;

    public string seed;
    public bool useRandomSeed;

    private Texture2D minimapTex;
    private Texture2D fogOfWarTex;

    private void Awake()
    {
        instance = this;
        Events.instance.PlayerTankDestroyed += GameOver;
        tilemap = GetComponentInChildren<Tilemap>();
        grid = tilemap.GetComponent<Grid>();

        InitTiles();
    }
    private void InitTiles()
    {
        object[] tiles = Resources.LoadAll("Tiles/BGTiles");
        foreach(object t in tiles)
        {
            Tile tile = (Tile) t;
            spawnableTiles.Add(tile);
        }
    }
    private void Start()
    {
        minimapTex = new Texture2D(chunkWidth, chunkHeight);
        fogOfWarTex = new Texture2D(chunkWidth, chunkHeight);
        GenerateMap();
        if (playerTankConstellationFromSelectScreen)
        {
            Ref.PCon.TGeo._tankRoomConstellation = playerTankConstellationFromSelectScreen;
        }
        Ref.PCon.SpawnTank();
    }
    private void Update()
    {
        if (Ref.PlayerGO)
        {
            playerPosRelativeToGrid = grid.WorldToCell(Ref.PlayerGO.transform.position);
        }
        TrackPlayerInMinimap();
    }

    //  Random Gen

    private void GenerateMap()
    {
        map = new int[chunkWidth, chunkHeight];
        RandomFillMap();

        int iterations = 5;

        for (int i = 0; i < iterations; i++)
        {
            SmoothMap();
        }
        Vector2Int centeringVector = -1 * new Vector2Int(chunkWidth/2, chunkHeight/2);
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
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkWidth; y++)
            {
                Tile t = Resources.Load("Tiles/BGTiles/StoneTile", typeof(Tile)) as Tile;
                if (map[x, y] == 0)
                {
                    object[] grassTiles = Resources.LoadAll("Tiles/BGTiles/GrassTiles");
                    int index = UnityEngine.Random.Range(0, grassTiles.Length);
                    t = (Tile) grassTiles[index];
                }
                else if(map[x, y] == 1) t = Resources.Load("Tiles/BGTiles/StoneTile", typeof(Tile)) as Tile;

                tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), t);
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
                int neighbourWallTiles = GetSurroundingWallsCount(x,y);

                if(neighbourWallTiles > threshold)
                {
                    map[x, y] = 1;
                }
                else if(neighbourWallTiles < threshold)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    private int GetSurroundingWallsCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for(int neighbourX = gridX-1; neighbourX <= gridX+1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if(neighbourX >= 0 && neighbourX < chunkWidth && neighbourY >= 0 && neighbourY < chunkHeight)
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

                if(map[x,y] == 1) minimapTex.SetPixel(x,y, StoneColor);
                else minimapTex.SetPixel(x, y, GrassColor);
                fogOfWarTex.SetPixel(x,y, Color.grey);
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
        Ref.UI._bigMapRect.sizeDelta = new Vector2(chunkWidth, chunkHeight);
        Ref.UI._bigMapImage.sprite = minimapSprite;

        Ref.UI._fogOfWarRect.sizeDelta = new Vector2(chunkWidth, chunkHeight);
        Ref.UI._fogOfWarImage.sprite = Sprite.Create(fogOfWarTex,
                                             new Rect(0.0f, 0.0f, fogOfWarTex.width, fogOfWarTex.height),
                                             new Vector2(0.5f, 0.5f), 
                                             100.0f);
    }
    private void TrackPlayerInMinimap()
    {
        float scale = 2;
        Ref.UI._bigMapRect.anchoredPosition = -1 * Ref.PCon.transform.position * scale;
        UpdateFogOfWar(-1 * Ref.UI._bigMapRect.anchoredPosition);
    }
    private void UpdateFogOfWar(Vector2 playerPos)
    {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(playerPos.x), Mathf.RoundToInt(playerPos.y));

        float ratio = Screen.width / (float)Screen.height;
        int visionRadiusWidth = Mathf.FloorToInt(20 * ratio);
        int visionRadiusHeight = Mathf.FloorToInt(20 / ratio);
        int radius = Math.Max(visionRadiusWidth, visionRadiusHeight);

        DrawCircle(fogOfWarTex, pos.x, pos.y, radius);
        Ref.UI._fogOfWarImage.sprite = Sprite.Create(fogOfWarTex,
                                             new Rect(0.0f, 0.0f, fogOfWarTex.width, fogOfWarTex.height),
                                             new Vector2(0.5f, 0.5f),
                                             100.0f);
    }

    private void DrawCircle(Texture2D tex, int x, int y, int radius)
    {
        float rSquared = radius * radius;
        x += tex.width / 2;
        y += tex.height / 2;

        for (int u = x - radius; u < x + radius + 1; u++)
            for (int v = y - radius; v < y + radius + 1; v++)
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                    tex.SetPixel(u, v, Color.clear);

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

    private void CreateTilemap(int radiusX, int radiusY, Vector2 vec)
    {
        int startPosX = Mathf.FloorToInt(vec.x);
        int startPosY = Mathf.FloorToInt(vec.y);

        for (int x = -radiusX; x < radiusX; x++)
        {
            for (int y = -radiusY; y < radiusY; y++)
            {
                if (tilemap.GetTile(new Vector3Int(x + startPosX, y + startPosY, 0)) == null)
                {
                    tilemap.SetTile(new Vector3Int(x + startPosX, y + startPosY, 0), spawnableTiles[UnityEngine.Random.Range(0, spawnableTiles.Count-1)]);
                }
            }
        }
    }
    private void GameOver()
    {
        Ref.UI.SpawnGameOverScreen();
    }
    public void GoToMainMenu()
    {
        StartCoroutine(ShowLoadingScreen());
    }
    public IEnumerator ShowLoadingScreen()
    {
        Instantiate((GameObject)Resources.Load("LoadingScreen"));
        Time.timeScale = 1;
        yield return new WaitForSeconds(2f);
        Loader.Load(Loader.Scene.MenuScene);
    }
}
