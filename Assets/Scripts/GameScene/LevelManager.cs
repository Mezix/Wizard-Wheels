using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static TankRoomConstellation playerTankConstellationFromSelectScreen;

    private Tilemap tm;
    private Grid g;
    private Vector3 playerPosRelativeToGrid;

    List<Tile> AllTiles = new List<Tile>();

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
    private void Awake()
    {
        instance = this;
        Events.instance.PlayerTankDestroyed += GameOver;
        tm = GetComponentInChildren<Tilemap>();
        g = tm.GetComponent<Grid>();

        InitTiles();
    }
    private void InitTiles()
    {
        object[] tiles = Resources.LoadAll("Tiles/BGTiles");
        foreach(object t in tiles)
        {
            Tile tile = (Tile) t;
            AllTiles.Add(tile);
        }
    }
    private void Start()
    {
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
            playerPosRelativeToGrid = g.WorldToCell(Ref.PlayerGO.transform.position);
            //CreateTilemap(35, 35, playerPosRelativeToGrid);
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

                tm.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), t);
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
        Texture2D minimapTex = new Texture2D(chunkWidth, chunkHeight);
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkWidth; y++)
            {
                if(map[x,y] == 1) minimapTex.SetPixel(x,y, StoneColor);
                else minimapTex.SetPixel(x, y, GrassColor);
            }
        }
        minimapTex.Apply();

        Sprite minimapSprite = Sprite.Create(minimapTex, 
                                             new Rect(0.0f, 0.0f, minimapTex.width, minimapTex.height), 
                                             new Vector2(0.5f, 0.5f), 100.0f);

        Ref.UI._bigMapRect.sizeDelta = new Vector2(chunkWidth, chunkHeight);
        Ref.UI._bigMapImage.sprite = minimapSprite;
    }
    private void TrackPlayerInMinimap()
    {
        float scale = 2;
        Ref.UI._bigMapRect.anchoredPosition = -1 * Ref.PCon.transform.position * scale;
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
                if (tm.GetTile(new Vector3Int(x + startPosX, y + startPosY, 0)) == null)
                {
                    tm.SetTile(new Vector3Int(x + startPosX, y + startPosY, 0), AllTiles[UnityEngine.Random.Range(0, AllTiles.Count-1)]);
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
