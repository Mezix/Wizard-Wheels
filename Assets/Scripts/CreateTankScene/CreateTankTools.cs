using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
public class CreateTankTools : MonoBehaviour
{
    public Grid _tempFloorGrid;
    private Tilemap tempFloorTilemap;

    public Grid _tempRoofGrid;
    private Tilemap tempRoofTilemap;

    private bool brushing;
    public Dropdown _toolSelecterDropdown;

    public bool previewTile;
    private GameObject tirePreview;
    private GameObject wallPreview;

    CreateTankUI ui;

    public void Awake()
    {
        tempFloorTilemap = _tempFloorGrid.GetComponent<Tilemap>();
        tempRoofTilemap = _tempRoofGrid.GetComponent<Tilemap>();
    }
    private void Start()
    {
        brushing = true;
        previewTile = true;
        ui = CreateTankSceneManager.instance._tUI;
    }
    void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }
    private void HandleMouseInput()
    {
        tempFloorTilemap.ClearAllTiles();
        tempRoofTilemap.ClearAllTiles();
        if (wallPreview) Destroy(wallPreview);
        if (tirePreview) Destroy(tirePreview);

        Tilemap currentlySelectedTempTilemap;
        Tilemap currentlySelectedTilemap;
        int tileType = CreateTankSceneManager.instance._tUI._tileTypeIndex;
        if (tileType == 0)
        {
            currentlySelectedTempTilemap = tempFloorTilemap;
            currentlySelectedTilemap = CreateTankSceneManager.instance._tGeo.FloorTilemap;
        }
        else
        {
            currentlySelectedTempTilemap = tempRoofTilemap;
            currentlySelectedTilemap = CreateTankSceneManager.instance._tGeo.RoofTilemap;
        }

        RaycastHit2D hit = HM.RaycastToMouseCursor();
        Vector3Int tempCellPos = currentlySelectedTempTilemap.WorldToCell(hit.point);

        //Show the currently selected tile were painting with on the temporary tilemap

        if (brushing)
        {
            if(tileType == 0 || tileType == 1) currentlySelectedTempTilemap.SetTile(tempCellPos, ui.GetTile());
            else if (tileType == 2)
            {
                Vector3 wallPos = tempFloorTilemap.CellToWorld(tempCellPos);
                HoverWall(wallPos);
                currentlySelectedTempTilemap.SetTile(tempCellPos, Resources.Load("Art/Tilemap Assets/EraserTile", typeof(Tile)) as Tile);
            }
            else if (tileType == 3)
            {
                Vector3 tirepos = tempFloorTilemap.CellToWorld(tempCellPos);
                HoverTire(tirepos);
                currentlySelectedTempTilemap.SetTile(tempCellPos, Resources.Load("Art/Tilemap Assets/EraserTile", typeof(Tile)) as Tile);
            }
        }
        else currentlySelectedTempTilemap.SetTile(tempCellPos, Resources.Load("Art/Tilemap Assets/EraserTile", typeof (Tile)) as Tile);

        if (Input.GetKey(KeyCode.Mouse0) && !MouseCursor.IsPointerOverUIElement())
        {
            Vector3Int cellpos = currentlySelectedTilemap.WorldToCell(hit.point);
            Vector2Int pos = CreateTankSceneManager.instance._tGeo.TilemapToCellPos(cellpos);

            //paint on the correct tilemap
            if (brushing)
            {
                if (tileType == 0)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeFloorAtPos(pos.x, pos.y, 1, 1, ui.GetTile());
                }
                else if (tileType == 1)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeRoofAtPos(pos.x, pos.y, 1, 1, ui.GetTile());
                }
                else if (tileType == 2)
                {
                    string tileDirection = "";
                    if (ui.wallIndex == 0) tileDirection = "up";
                    if (ui.wallIndex == 1) tileDirection = "left";
                    if (ui.wallIndex == 2) tileDirection = "down";
                    if (ui.wallIndex == 3) tileDirection = "right";
                    CreateTankSceneManager.instance._tGeo.CreateWallAtPos(pos.x, pos.y, tileDirection);
                }
                else if (tileType == 3)
                {
                }
            }

            //erase from the correct tilemap
            else
            {
                if (tileType == 0)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeFloorAtPos(pos.x, pos.y, 1, 1, null);
                }
                else if (tileType == 1)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeRoofAtPos(pos.x, pos.y, 1, 1, null);
                }
                else if (tileType == 2)
                {
                }
                else if (tileType == 3)
                {
                }
            }
        }
    }
    private void HoverWall(Vector3 wallPos)
    {
        wallPreview = Instantiate(ui._wallsGOList[ui.wallIndex]);
        wallPreview.transform.position = wallPos + new Vector3(0, 0.5f, 0);
    }
    private void HoverTire(Vector3 tirePos)
    {
        tirePreview = Instantiate(ui._tiresGOList[ui.tiresIndex]);
        tirePreview.transform.position = tirePos + new Vector3(0, 0.5f, 0);
    }
    private void HandleKeyboardInput()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            SelectBrush();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            SelectEraser();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ui.SelectList(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ui.SelectList(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ui.SelectList(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ui.SelectList(3);
        }
    }
    public void SelectTool(int value)
    {
        if (value == 0)
        {
            SelectBrush();
        }
        else if (value == 1)
        {
            SelectEraser();
        }
    }
    private void SelectBrush()
    {
        brushing = true;
        previewTile = false;
        CreateTankSceneManager.instance.mouse._mouseState = "Brush";
    }
    private void SelectEraser()
    {
        brushing = false;
        previewTile = false;
        CreateTankSceneManager.instance.mouse._mouseState = "Eraser";
    }
}
