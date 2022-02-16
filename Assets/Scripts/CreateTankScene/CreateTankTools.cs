using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CreateTankTools : MonoBehaviour
{
    [HideInInspector]
    public Grid tempGrid;
    [HideInInspector]
    public Tilemap tempTilemap;
    private bool brushing;
    public void Awake()
    {
        tempGrid = GetComponentInChildren<Grid>();
        tempTilemap = GetComponentInChildren<Tilemap>();
    }
    private void Start()
    {
        brushing = true;
    }
    void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        tempTilemap.ClearAllTiles();

        RaycastHit2D hit = HM.RaycastToMouseCursor();
        Vector3Int cellpos = tempTilemap.WorldToCell(hit.point);
        CreateTankUI ui = CreateTankSceneManager.instance._tUI;
        tempTilemap.SetTile(cellpos, ui._useableTiles[ui.currentTileIndex]);

        if(Input.GetKey(KeyCode.Mouse0))
        {
            Tilemap currentlySelectedTilemap = CreateTankSceneManager.instance._tGeo.FloorTilemap;
            cellpos = currentlySelectedTilemap.WorldToCell(hit.point);
            if (brushing)
            {
                currentlySelectedTilemap.SetTile(cellpos, ui._useableTiles[ui.currentTileIndex]);
            }
            else
            {
                currentlySelectedTilemap.SetTile(cellpos, null);
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

    private void SelectBrush()
    {
        brushing = true;
    }

    private void SelectEraser()
    {
        brushing = false;
    }
}
