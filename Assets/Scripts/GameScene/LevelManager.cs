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
        object[] tiles = Resources.LoadAll("Tiles\\BGTiles");
        foreach(object t in tiles)
        {
            Tile tile = (Tile) t;
            AllTiles.Add(tile);
        }
    }

    private void Start()
    {
        if (playerTankConstellationFromSelectScreen)
        {
            Ref.PCon.TGeo._tankRoomConstellation = playerTankConstellationFromSelectScreen;
        }
        Ref.PCon.SpawnTank();
    }
    private void Update()
    {
        if(Ref.PlayerGO)
        {
            playerPosRelativeToGrid = g.WorldToCell(Ref.PlayerGO.transform.position);
            CreateTilemap(35, 35, playerPosRelativeToGrid);
        }
    }

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
                    tm.SetTile(new Vector3Int(x + startPosX, y + startPosY, 0), AllTiles[Random.Range(0, AllTiles.Count-1)]);
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
