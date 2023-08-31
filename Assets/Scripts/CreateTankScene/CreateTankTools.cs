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

    //  Rotation
    private Vector2 _rotationStartPos = Vector2.zero;
    public SystemRotationArrow _arrow;

    public void Awake()
    {
        selectedObjectPositionPreview = Instantiate(Resources.Load("Prefabs/Data Manipulation Scene/SelectedObjectPositionPreview", typeof(GameObject)) as GameObject, _alignmentGrid.transform.GetChild(0));
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
        if (floorPreview) Destroy(floorPreview);
        if (roofPreview) Destroy(roofPreview);
        if (wallPreview) Destroy(wallPreview);
        if (tirePreview) Destroy(tirePreview);
        if (systemPreview) Destroy(systemPreview);

        int tileType = CreateTankSceneManager.instance._tUI._partTypeIndex;
        Vector3Int alignmentCellPos = _alignmentGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int cellPos = new Vector3Int(alignmentCellPos.x, -1 + -alignmentCellPos.y);
        Vector3 previewPos = new Vector3(alignmentCellPos.x / 2f, alignmentCellPos.y / 2f) + new Vector3(0, 0.5f, 0);

        selectedObjectPositionPreview.transform.localPosition = previewPos;

        if (brushing)
        {
            if (tileType == 0) HoverFloor(previewPos);
            else if (tileType == 1) HoverRoof(previewPos);
            else if (tileType == 2) HoverWall(previewPos);
            else if (tileType == 3) HoverTire(previewPos);
            else if (tileType == 4) HoverSystem(previewPos);
        }

        if (Input.GetKey(KeyCode.Mouse0) && !MouseCursor.IsPointerOverUIElement())
        {
            int amountOfRows = CreateTankSceneManager.instance._tGeo._roomPosMatrix.GetLength(0);
            int amountOfColumns = CreateTankSceneManager.instance._tGeo._roomPosMatrix.GetLength(1);

            if (cellPos.x < 0 || cellPos.x >= amountOfRows || cellPos.y < 0 || cellPos.y >= amountOfColumns)
            {
                Debug.LogWarning("Trying to work outside of bounds of matrix");
                return;
            }

            //paint on the correct tilemap
            if (brushing)
            {
                if (tileType == 0)
                {
                    if (CreateTankSceneManager.instance._tGeo._roomPosMatrix[cellPos.x, cellPos.y] == null)
                        CreateTankSceneManager.instance._tGeo.CreateRoomAtPos(cellPos.x, cellPos.y, Resources.Load(GS.RoomPrefabs("1x1Room"), typeof(GameObject)) as GameObject);

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
                if (CreateTankSceneManager.instance._tGeo._roomPosMatrix[cellPos.x, cellPos.y] == null)
                {
                    Debug.LogWarning("Room doesn't exist, nothing to delete!");
                    return;
                }

                if (tileType == 0)
                {
                    CreateTankSceneManager.instance._tGeo.CreateRoomAtPos(cellPos.x, cellPos.y, null);
                }
                else if (tileType == 1)
                {
                    CreateTankSceneManager.instance._tGeo.ChangeRoofAtPos(cellPos.x, cellPos.y, PlayerData.RoofType.RoofA);
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

    //  Hover
    private void HoverFloor(Vector3 pos)
    {
        floorPreview = Instantiate(Resources.Load("Prefabs/Data Manipulation Scene/FloorPreview", typeof(GameObject)) as GameObject, _alignmentGrid.transform.GetChild(0));
        floorPreview.transform.localPosition = pos;

        SpriteRenderer sr = floorPreview.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load(GS.RoomGraphics(CreateTankSceneManager.instance._tUI._floorTypes[CreateTankSceneManager.instance._tUI.floorIndex].ToString()) + "3", typeof(Sprite)) as Sprite;
        sr.color = ui._selectedColor;
    }
    private void HoverRoof(Vector3 pos)
    {
        roofPreview = Instantiate(Resources.Load("Prefabs/Data Manipulation Scene/RoofPreview", typeof(GameObject)) as GameObject, _alignmentGrid.transform.GetChild(0));
        roofPreview.transform.localPosition = pos;

        SpriteRenderer sr = roofPreview.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load(GS.RoomGraphics(CreateTankSceneManager.instance._tUI._roofTypes[CreateTankSceneManager.instance._tUI.roofIndex].ToString()), typeof(Sprite)) as Sprite;
        sr.color = ui._selectedColor;
    }
    private void HoverWall(Vector3 pos)
    {
        wallPreview = Instantiate(ui._wallsGOList[ui.wallIndex], _alignmentGrid.transform.GetChild(0));
        wallPreview.transform.localPosition = pos;
    }
    private void HoverTire(Vector3 pos)
    {
        tirePreview = Instantiate(ui._tiresGOList[ui.tiresIndex], _alignmentGrid.transform.GetChild(0));
        tirePreview.transform.localPosition = pos;
    }
    private void HoverSystem(Vector3 pos)
    {
        systemPreview = Instantiate(ui._systemGOList[ui.systemsIndex], _alignmentGrid.transform.GetChild(0));
        systemPreview.transform.localPosition = pos;

        ASystem system = systemPreview.GetComponent<ASystem>();
        system._direction = Enum.GetValues(typeof(ASystem.DirectionToSpawnIn)).Cast<ASystem.DirectionToSpawnIn>().ToList()[CreateTankSceneManager.instance._tUI._directionDropDown.value];
        system.SpawnInCorrectDirection();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SelectBrush();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectEraser();
        }

        _arrow._arrowParents.SetActive(false);
        if (Input.GetKeyDown(KeyCode.R))
        {
            _rotationStartPos = Input.mousePosition;
            _arrow.transform.position = Vector3.forward * 10 + Camera.main.ScreenToWorldPoint(_rotationStartPos);
        }
        if (Input.GetKey(KeyCode.R)) ChangeSystemDirection();

        // Select which parts to spawn
        if (Input.GetKeyDown(KeyCode.Alpha1)) ui.SelectList(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ui.SelectList(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ui.SelectList(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ui.SelectList(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ui.SelectList(4);
    }

    private void ChangeSystemDirection()
    {
        _arrow._arrowParents.gameObject.SetActive(true);
        float actualAngle = HM.UnwrapAngle(HM.GetAngle2DBetween(_rotationStartPos, Input.mousePosition) + 180);
        HM.RotateLocalTransformToAngle(_arrow._actualRotationArrow, new Vector3(0, 0, actualAngle));

        float sysDirectionAngle = 0;
        if (actualAngle >= -45 && actualAngle < 45) { ui._directionDropDown.value = 0; sysDirectionAngle = 0; }
        else if (actualAngle >= 45 && actualAngle < 135) { ui._directionDropDown.value = 3; sysDirectionAngle = 90; }
        else if (actualAngle >= 135 && actualAngle < 225) { ui._directionDropDown.value = 2; sysDirectionAngle = 180; }
        else if (actualAngle >= 225 && actualAngle < 315) { ui._directionDropDown.value = 1; sysDirectionAngle = 270; }
        else { ui._directionDropDown.value = 0; sysDirectionAngle = 0; }
        HM.RotateLocalTransformToAngle(_arrow._systemRotationArrow, new Vector3(0, 0, sysDirectionAngle));
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
