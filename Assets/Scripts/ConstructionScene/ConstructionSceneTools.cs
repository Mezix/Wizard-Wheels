using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;

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

    public void Awake()
    {
        instance = this;
        selectedObjectPositionPreview = Instantiate(Resources.Load("Prefabs/Data Manipulation Scene/SelectedObjectPositionPreview", typeof(GameObject)) as GameObject, _alignmentGrid.transform.GetChild(0));
    }
    private void Start()
    {
        brushing = true;
        shouldPreviewTile = true;
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
        if (Input.GetKey(KeyCode.LeftShift)) return; //exclusion with camera movement using the shift key
        if (Input.mouseScrollDelta.y > 0)
        {
            ConstructionSceneUI.instance.NextItemInList();
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            ConstructionSceneUI.instance.PreviousItemInList();
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
        //Vector3Int alignmentCellPos = _alignmentGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //Vector3Int cellPos = new Vector3Int(alignmentCellPos.x, -1 + -alignmentCellPos.y);
        //Vector3 previewPos = new Vector3(alignmentCellPos.x / 2f, alignmentCellPos.y / 2f) + new Vector3(0, 0.5f, 0);

        //selectedObjectPositionPreview.transform.localPosition = previewPos;

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
                    string tileDirection = "";
                    if (ConstructionSceneUI.instance.wallIndex == 0) tileDirection = "up";
                    if (ConstructionSceneUI.instance.wallIndex == 1) tileDirection = "left";
                    if (ConstructionSceneUI.instance.wallIndex == 2) tileDirection = "down";
                    if (ConstructionSceneUI.instance.wallIndex == 3) tileDirection = "right";
                    ConstructionSceneGeometry.instance.CreateWallAtPos(cellPos.x, cellPos.y, tileDirection);
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
                    ConstructionSceneGeometry.instance.CreateWallAtPos(cellPos.x, cellPos.y, "delete");
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
        wallPreview = Instantiate(ConstructionSceneUI.instance._wallsGOList[ConstructionSceneUI.instance.wallIndex], _alignmentGrid.transform.GetChild(0));
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
        system._direction = Enum.GetValues(typeof(ASystem.DirectionToSpawnIn)).Cast<ASystem.DirectionToSpawnIn>().ToList()[ConstructionSceneUI.instance._directionDropDown.value];
        system.SpawnInCorrectDirection();
    }
    private void ChangeSystemDirection()
    {
        _arrow._arrowParents.gameObject.SetActive(true);
        float actualAngle = HM.UnwrapAngle(HM.GetEulerAngle2DBetween(_arrow.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) + 180);
        HM.RotateLocalTransformToAngle(_arrow._actualRotationArrow, new Vector3(0, 0, actualAngle));

        float sysDirectionAngle = 0;
        if (actualAngle >= -45 && actualAngle < 45) { ConstructionSceneUI.instance._directionDropDown.value = 0; sysDirectionAngle = 0; }
        else if (actualAngle >= 45 && actualAngle < 135) { ConstructionSceneUI.instance._directionDropDown.value = 3; sysDirectionAngle = 90; }
        else if (actualAngle >= 135 && actualAngle < 225) { ConstructionSceneUI.instance._directionDropDown.value = 2; sysDirectionAngle = 180; }
        else if (actualAngle >= 225 && actualAngle < 315) { ConstructionSceneUI.instance._directionDropDown.value = 1; sysDirectionAngle = 270; }
        else { ConstructionSceneUI.instance._directionDropDown.value = 0; sysDirectionAngle = 0; }
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
        ConstructionSceneMouse.instance._mouseState = "Brush";
    }
    private void SelectEraser()
    {
        brushing = false;
        shouldPreviewTile = false;
        ConstructionSceneMouse.instance._mouseState = "Eraser";
    }
}
