using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;
using static ASystem;
using static ConstructionSceneMouse;

public class ConstructionSceneTools : MonoBehaviour
{
    public static ConstructionSceneTools instance;

    private bool brushing;
    public Dropdown _toolSelecterDropdown;

    public bool shouldPreviewTile;
    private GameObject wallPreview;
    private GameObject floorPreview;
    private GameObject roofPreview;
    private GameObject tirePreview;
    private GameObject systemPreview;
    private GameObject selectedObjectPositionPreview;

    //  Cell Pos

    public Grid _alignmentGrid;
    Vector3Int alignmentCellPos;
    Vector3Int cellPos;
    Vector3 previewPos;

    //  Rotation
    public SystemRotationArrow _arrow;
    public DirectionToSpawnIn systemSpawnDirection;

    //  Expander
    public VehicleExpanderWidget _expanderWidget;
    public int expanderModifier = 1;

    public void Awake()
    {
        instance = this;
        selectedObjectPositionPreview = Instantiate(Resources.Load(GS.DataScenePrefabs("SelectedObjectPositionPreview"), typeof(GameObject)) as GameObject, _alignmentGrid.transform.GetChild(0));
    }
    private void Start()
    {
        brushing = true;
        shouldPreviewTile = true;
        InitWidget();
    }

    private void InitWidget()
    {
        _expanderWidget._left.onClick.AddListener(() => ConstructionSceneGeometry.instance.ModifyVehicleSize(1 * expanderModifier, 0, 0, 0));
        _expanderWidget._right.onClick.AddListener(() => ConstructionSceneGeometry.instance.ModifyVehicleSize(0, 1 * expanderModifier, 0, 0));
        _expanderWidget._top.onClick.AddListener(() => ConstructionSceneGeometry.instance.ModifyVehicleSize(0, 0, 1 * expanderModifier, 0));
        _expanderWidget._down.onClick.AddListener(() => ConstructionSceneGeometry.instance.ModifyVehicleSize(0, 0, 0, 1 * expanderModifier));
    }

    void Update()
    {
        HandleScrollWheel();
        GetCellPositionAndRotation();
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleScrollWheel()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                ConstructionSceneUI.instance.NextItemInList();
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                ConstructionSceneUI.instance.PreviousItemInList();
            }
        }
    }
    private void GetCellPositionAndRotation()
    {
        _arrow._arrowParents.SetActive(false);
        if (Input.GetKeyDown(KeyCode.R))
        {
            alignmentCellPos = _alignmentGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            _arrow.transform.localPosition = Vector3.forward * 10 + new Vector3(alignmentCellPos.x / 2f, alignmentCellPos.y / 2f) + new Vector3(0.25f, 0.25f, 0);
        }
        if (Input.GetKey(KeyCode.R)) ChangeSystemDirection();
        else
        { 
            alignmentCellPos = _alignmentGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        cellPos = new Vector3Int(alignmentCellPos.x, -1 + -alignmentCellPos.y);
        previewPos = new Vector3(alignmentCellPos.x / 2f, alignmentCellPos.y / 2f) + new Vector3(0, 0.5f, 0);

        if (MouseCursor.IsPointerOverUIElement()) selectedObjectPositionPreview.SetActive(false);
        else selectedObjectPositionPreview.SetActive(true);
        selectedObjectPositionPreview.transform.localPosition = previewPos;
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
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _expanderWidget.SetWidgetDirection(false);
            expanderModifier = -1;
        }
        else
        {
            _expanderWidget.SetWidgetDirection(true);
            expanderModifier = 1;
        }

        // Select which parts to spawn
        if (Input.GetKeyDown(KeyCode.Alpha1)) ConstructionSceneUI.instance.SelectList(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ConstructionSceneUI.instance.SelectList(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ConstructionSceneUI.instance.SelectList(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ConstructionSceneUI.instance.SelectList(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ConstructionSceneUI.instance.SelectList(4);
    }
    private void HandleMouseInput()
    {
        if (floorPreview) Destroy(floorPreview);
        if (roofPreview) Destroy(roofPreview);
        if (wallPreview) Destroy(wallPreview);
        if (tirePreview) Destroy(tirePreview);
        if (systemPreview) Destroy(systemPreview);

        int tileType = ConstructionSceneUI.instance._partTypeIndex;
        if (MouseCursor.IsPointerOverUIElement()) return;

        if (brushing)
        {
            if (tileType == 0) HoverFloor(previewPos);
            else if (tileType == 1) HoverRoof(previewPos);
            else if (tileType == 2) HoverWall(previewPos);
            else if (tileType == 3) HoverTire(previewPos);
            else if (tileType == 4) HoverSystem(previewPos);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            int amountOfRows = ConstructionSceneGeometry.instance._roomPosMatrix.GetLength(0);
            int amountOfColumns = ConstructionSceneGeometry.instance._roomPosMatrix.GetLength(1);

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
                    if (ConstructionSceneGeometry.instance._roomPosMatrix[cellPos.x, cellPos.y] == null)
                        ConstructionSceneGeometry.instance.CreateRoomAtPos(cellPos.x, cellPos.y, Resources.Load(GS.RoomPrefabs("1x1Room"), typeof(GameObject)) as GameObject);

                    ConstructionSceneGeometry.instance.ChangeFloorAtPos(cellPos.x, cellPos.y, ConstructionSceneUI.instance._floorTypes[ConstructionSceneUI.instance.floorIndex]);
                }
                else if (tileType == 1)
                {
                    if (ConstructionSceneGeometry.instance._roomPosMatrix[cellPos.x, cellPos.y] == null)
                        ConstructionSceneGeometry.instance.CreateRoomAtPos(cellPos.x, cellPos.y, Resources.Load(GS.RoomPrefabs("1x1Room"), typeof(GameObject)) as GameObject);

                    ConstructionSceneGeometry.instance.ChangeRoofAtPos(cellPos.x, cellPos.y, ConstructionSceneUI.instance._roofTypes[ConstructionSceneUI.instance.roofIndex]);
                }

                if (ConstructionSceneGeometry.instance._roomPosMatrix[cellPos.x, cellPos.y] == null)
                {
                    Debug.LogWarning("Room doesn't exist, cant create any objects here!");
                    return;
                }
                else if (tileType == 2)
                {
                    ConstructionSceneGeometry.instance.CreateWallAtPos(cellPos.x, cellPos.y, systemSpawnDirection);
                }
                else if (tileType == 3)
                {
                    ConstructionSceneGeometry.instance.CreateTireAtPos(cellPos.x, cellPos.y, ConstructionSceneUI.instance.GetTirePrefab());
                }
                else if (tileType == 4)
                {
                    ConstructionSceneGeometry.instance.CreateSystemAtPos(cellPos.x, cellPos.y, ConstructionSceneUI.instance.GetSystemPrefab());
                }
            }

            //erase from the correct tilemap
            else
            {
                if (ConstructionSceneGeometry.instance._roomPosMatrix[cellPos.x, cellPos.y] == null)
                {
                    Debug.LogWarning("Room doesn't exist, nothing to delete!");
                    return;
                }

                if (tileType == 0)
                {
                    ConstructionSceneGeometry.instance.CreateRoomAtPos(cellPos.x, cellPos.y, null);
                }
                else if (tileType == 1)
                {
                    ConstructionSceneGeometry.instance.ChangeRoofAtPos(cellPos.x, cellPos.y, PlayerData.RoofType.RoofA);
                }
                else if (tileType == 2)
                {
                    ConstructionSceneGeometry.instance.CreateWallAtPos(cellPos.x, cellPos.y, DirectionToSpawnIn.Right, true);
                }
                else if (tileType == 3)
                {
                    ConstructionSceneGeometry.instance.CreateTireAtPos(cellPos.x, cellPos.y, null);
                }
                else if (tileType == 4)
                {
                    ConstructionSceneGeometry.instance.CreateSystemAtPos(cellPos.x, cellPos.y, null);
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
        sr.sprite = Resources.Load(GS.RoomGraphics(ConstructionSceneUI.instance._floorTypes[ConstructionSceneUI.instance.floorIndex].ToString()) + "3", typeof(Sprite)) as Sprite;
        sr.color = ConstructionSceneUI.instance._selectedColor;
    }
    private void HoverRoof(Vector3 pos)
    {
        roofPreview = Instantiate(Resources.Load("Prefabs/Data Manipulation Scene/RoofPreview", typeof(GameObject)) as GameObject, _alignmentGrid.transform.GetChild(0));
        roofPreview.transform.localPosition = pos;

        SpriteRenderer sr = roofPreview.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load(GS.RoomGraphics(ConstructionSceneUI.instance._roofTypes[ConstructionSceneUI.instance.roofIndex].ToString()), typeof(Sprite)) as Sprite;
        sr.color = ConstructionSceneUI.instance._selectedColor;
    }
    private void HoverWall(Vector3 pos)
    {
        GameObject wallToSpawn = Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject;
        if (systemSpawnDirection.Equals(DirectionToSpawnIn.Left)) wallToSpawn = Resources.Load(GS.WallPrefabs("WallLeft"), typeof(GameObject)) as GameObject;
        else if(systemSpawnDirection.Equals(DirectionToSpawnIn.Right)) wallToSpawn = Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject;
        else if (systemSpawnDirection.Equals(DirectionToSpawnIn.Down)) wallToSpawn = Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject;

        wallPreview = Instantiate(wallToSpawn, _alignmentGrid.transform.GetChild(0));
        wallPreview.transform.localPosition = pos;
    }
    private void HoverTire(Vector3 pos)
    {
        tirePreview = Instantiate(ConstructionSceneUI.instance._tiresGOList[ConstructionSceneUI.instance.tiresIndex], _alignmentGrid.transform.GetChild(0));
        tirePreview.transform.localPosition = pos;
    }
    private void HoverSystem(Vector3 pos)
    {
        systemPreview = Instantiate(ConstructionSceneUI.instance._systemGOList[ConstructionSceneUI.instance.systemsIndex], _alignmentGrid.transform.GetChild(0));
        systemPreview.transform.localPosition = pos;

        ASystem system = systemPreview.GetComponent<ASystem>();
        if (system.TryGetComponent(out AWeapon wep)) Destroy(wep.WeaponUI.gameObject);
        system._direction = systemSpawnDirection;
        system.SpawnInCorrectDirection();
    }
    private void ChangeSystemDirection()
    {
        _arrow._arrowParents.gameObject.SetActive(true);
        float actualAngle = HM.UnwrapAngle(HM.GetEulerAngle2DBetween(_arrow.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) + 180);
        HM.RotateLocalTransformToAngle(_arrow._actualRotationArrow, new Vector3(0, 0, actualAngle));

        float sysDirectionAngle;
        if (actualAngle >= -45 && actualAngle < 45) { systemSpawnDirection = DirectionToSpawnIn.Right; sysDirectionAngle = 0; }
        else if (actualAngle >= 45 && actualAngle < 135) { systemSpawnDirection = DirectionToSpawnIn.Up; sysDirectionAngle = 90; }
        else if (actualAngle >= 135 && actualAngle < 225) { systemSpawnDirection = DirectionToSpawnIn.Left; sysDirectionAngle = 180; }
        else if (actualAngle >= 225 && actualAngle < 315) { systemSpawnDirection = DirectionToSpawnIn.Down; sysDirectionAngle = 270; }
        else { systemSpawnDirection = DirectionToSpawnIn.Right; sysDirectionAngle = 0; }
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
        shouldPreviewTile = false;
        ConstructionSceneMouse.instance._mouseState = MouseState.Brushing;
    }
    private void SelectEraser()
    {
        brushing = false;
        shouldPreviewTile = false;
        ConstructionSceneMouse.instance._mouseState = MouseState.Eraser;
    }
}
