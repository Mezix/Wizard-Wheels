using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CreateTankTools : MonoBehaviour
{
    public Grid _tempFloorGrid;
    private Tilemap tempFloorTilemap;

    public Grid _tempRoofGrid;
    private Tilemap tempRoofTilemap;

    private bool brushing;
    public bool previewTile;
    public void Awake()
    {
        tempFloorTilemap = _tempFloorGrid.GetComponent<Tilemap>();
        tempRoofTilemap = _tempRoofGrid.GetComponent<Tilemap>();
    }
    private void Start()
    {
        brushing = true;
        previewTile = true;
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

        CreateTankUI ui = CreateTankSceneManager.instance._tUI;
        if (brushing) currentlySelectedTempTilemap.SetTile(tempCellPos, ui.GetTile());
        else currentlySelectedTempTilemap.SetTile(tempCellPos, Resources.Load("Art/Tilemap Assets/EraserTile", typeof (Tile)) as Tile);

        if (Input.GetKeyDown(KeyCode.Mouse0) && !MouseCursor.IsPointerOverUIElement())
        {
            Vector3Int cellpos = currentlySelectedTilemap.WorldToCell(hit.point);
            Vector2Int pos = CreateTankSceneManager.instance._tGeo.TilemapToCellPos(cellpos);

            if (brushing)
            {

                //paint on the correct tilemap
                //currentlySelectedTilemap.SetTile(cellpos, ui.GetTile());

                //  Floor
                if (tileType == 0)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeFloorAtPos(pos.x, pos.y, 1, 1, ui.GetTile());
                }
            }
            else
            {
                //erase from the correct tilemap
                //currentlySelectedTilemap.SetTile(cellpos, null);

                if (tileType == 0)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeFloorAtPos(pos.x, pos.y, 1, 1, null);
                }
            }
        }
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
