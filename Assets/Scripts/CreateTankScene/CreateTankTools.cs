using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;

public class CreateTankTools : MonoBehaviour
{
    public Grid _alignmentGrid;
    private bool brushing;
    public Dropdown _toolSelecterDropdown;

    public bool previewTile;
    private GameObject wallPreview;
    private GameObject floorPreview;
    private GameObject roofPreview;
    private GameObject tirePreview;
    private GameObject systemPreview;
    private GameObject selectedObjectPositionPreview;
    private CreateTankUI ui;

    public void Awake()
    {
        selectedObjectPositionPreview = Instantiate(Resources.Load("Prefabs/Data Manipulation Scene/SelectedObjectPositionPreview", typeof(GameObject)) as GameObject);
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
        HandleScrollWheel();
    }

    private void HandleScrollWheel()
    {
        if (Input.GetKey(KeyCode.LeftShift)) return; //exclusion with camera movement using the shift key
        if (Input.mouseScrollDelta.y > 0)
        {
            ui.NextItemInList();
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            ui.PreviousItemInList();
        }
    }

    private void HandleMouseInput()
    {
        if (floorPreview)  Destroy(floorPreview);
        if (roofPreview)   Destroy(roofPreview);
        if (wallPreview)   Destroy(wallPreview);
        if (tirePreview)   Destroy(tirePreview);
        if (systemPreview) Destroy(systemPreview);

        int tileType = CreateTankSceneManager.instance._tUI._partTypeIndex;
        Vector3Int cellPos = _alignmentGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 previewPos = new Vector3(cellPos.x/2f-0.25f, cellPos.y/2f+0.5f, cellPos.z/2f);
        //Debug.Log(cellPos);
        cellPos.y = -cellPos.y;

        //Show the currently selected tile were painting with on the temporary tilemap

        selectedObjectPositionPreview.transform.position = previewPos;// + new Vector3(0, 0.5f, 0);

        if (brushing)
        {
            if (tileType == 0)
            {
                HoverFloor(previewPos);
            }
            if (tileType == 1)
            {
                HoverRoof(previewPos);
            }
            else if (tileType == 2)
            {
                HoverWall(previewPos);
            }
            else if (tileType == 3)
            {
                HoverTire(previewPos);
            }
            else if (tileType == 4)
            {
                HoverSystem(previewPos);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && !MouseCursor.IsPointerOverUIElement())
        {
            //paint on the correct tilemap
            if (brushing && CreateTankSceneManager.instance._tGeo._roomPosMatrix[cellPos.x, cellPos.y])
            {
                if (tileType == 0)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeFloorAtPos(cellPos.x, cellPos.y, CreateTankSceneManager.instance._tUI._floorTypes[CreateTankSceneManager.instance._tUI.floorIndex]);
                }
                else if (tileType == 1)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeRoofAtPos(cellPos.x, cellPos.y, CreateTankSceneManager.instance._tUI._roofTypes[CreateTankSceneManager.instance._tUI.roofIndex]);
                }
                else if (tileType == 2)
                {
                    string tileDirection = "";
                    if (ui.wallIndex == 0) tileDirection = "up";
                    if (ui.wallIndex == 1) tileDirection = "left";
                    if (ui.wallIndex == 2) tileDirection = "down";
                    if (ui.wallIndex == 3) tileDirection = "right";
                    CreateTankSceneManager.instance._tGeo.CreateWallAtPos(cellPos.x, cellPos.y, tileDirection);
                }
                else if (tileType == 3)
                {
                    CreateTankSceneManager.instance._tGeo.CreateTireAtPos(cellPos.x, cellPos.y, ui.GetTirePrefab());
                }
                else if (tileType == 4)
                {
                    CreateTankSceneManager.instance._tGeo.CreateSystemAtPos(cellPos.x, cellPos.y, ui.GetSystemPrefab());
                }
            }

            //erase from the correct tilemap
            else
            {
                if (tileType == 0)
                {
                   // CreateTankSceneManager.instance._tGeo.ChangeFloorAtPos(pos.x, pos.y, 1, 1, null);
                }
                else if (tileType == 1)
                {
                    //CreateTankSceneManager.instance._tGeo.ChangeRoofAtPos(pos.x, pos.y, 1, 1, null);
                }
                else if (tileType == 2)
                {
                    CreateTankSceneManager.instance._tGeo.CreateWallAtPos(cellPos.x, cellPos.y, "delete");
                }
                else if (tileType == 3)
                {
                    CreateTankSceneManager.instance._tGeo.CreateTireAtPos(cellPos.x, cellPos.y, null);
                }
                else if (tileType == 4)
                {
                    CreateTankSceneManager.instance._tGeo.CreateSystemAtPos(cellPos.x, cellPos.y, null);
                }
            }
        }
    }
    private void HoverFloor(Vector3 wallPos)
    {
        floorPreview = Instantiate(Resources.Load("Prefabs/Data Manipulation Scene/FloorPreview", typeof (GameObject)) as GameObject);
        floorPreview.transform.position = wallPos;
        SpriteRenderer sr = floorPreview.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load(GS.RoomGraphics(CreateTankSceneManager.instance._tUI._floorTypes[CreateTankSceneManager.instance._tUI.floorIndex].ToString())+"3", typeof (Sprite)) as Sprite;
        sr.color = ui._selectedColor;
    }
    private void HoverRoof(Vector3 wallPos)
    {
        roofPreview = Instantiate(Resources.Load("Prefabs/Data Manipulation Scene/RoofPreview", typeof(GameObject)) as GameObject);
        roofPreview.transform.position = wallPos;
        SpriteRenderer sr = roofPreview.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load(GS.RoomGraphics(CreateTankSceneManager.instance._tUI._roofTypes[CreateTankSceneManager.instance._tUI.roofIndex].ToString()), typeof(Sprite)) as Sprite;
        sr.color = ui._selectedColor;
    }
    private void HoverWall(Vector3 wallPos)
    {
        wallPreview = Instantiate(ui._wallsGOList[ui.wallIndex]);
        wallPreview.transform.position = wallPos;
    }
    private void HoverTire(Vector3 tirePos)
    {
        tirePreview = Instantiate(ui._tiresGOList[ui.tiresIndex]);
        tirePreview.transform.position = tirePos;
    }
    private void HoverSystem(Vector3 systemPos)
    {
        systemPreview = Instantiate(ui._systemGOList[ui.systemsIndex]);
        ASystem system = systemPreview.GetComponent<ASystem>();
        system._direction = Enum.GetValues(typeof(ASystem.DirectionToSpawnIn)).Cast<ASystem.DirectionToSpawnIn>().ToList()[CreateTankSceneManager.instance._tUI._directionDropDown.value];
        system.SpawnInCorrectDirection();
        systemPreview.transform.position = systemPos;
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
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ui.SelectList(4);
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
