using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AUnit;
using static PlayerData;

public class Room : MonoBehaviour
{
    public TankGeometry _tGeo;
    public int ID;
    public ASystem _roomSystem;
    public SpriteRenderer _roomSystemRenderer;
    public SpriteRenderer _floorRenderer;
    public SpriteRenderer _roofRenderer;
    public int _sizeX;
    public int _sizeY;

    public float _currentRepairStatus;
    public int _maxHP;
    public int _currentHP;
    public FloorType _floorType;
    public RoofType _roofType;

    public RoomPosition[] allRoomPositions;
    public RoomPosition[] freeRoomPositions;

    public List<AUnit> UnitsInRoom = new List<AUnit>();
    public RoomUI _roomUI;
    private void Awake()
    {
        InitRoomPositions();
    }

    //  Floor HP

    /// <summary>
    /// Returns true if the hull is completely repaired, otherwise false
    /// </summary>
    public bool RepairSlowly(float repairAmount)
    {
        if (_currentHP >= _maxHP) return true;

        _currentRepairStatus += repairAmount;
        if (_currentRepairStatus > 100)
        {
            _currentHP += 1;
            _currentRepairStatus = 0;
            UpdateDamage();
        }
        if(_tGeo.GetComponent<PlayerTankController>())
        {
            _roomUI.roomUIObjects.SetActive(true);
            _roomUI.UpdateRepair(_currentRepairStatus / 100f);
            _roomUI.NeedsUnitToRepair(false);
        }
        return false;
    }
    public void CheckRoomStillBeingRepaired()
    {
        bool AnyUnitRepairing = false;

        foreach(AUnit unit in UnitsInRoom)
        {
            if (unit._unitState.Equals(UnitState.Repairing)) { AnyUnitRepairing = true; break; }
        }
        if(AnyUnitRepairing)
        {
            _roomUI.NeedsUnitToRepair(false);
        }
        else
        {
            _roomUI.NeedsUnitToRepair(true);
        }
    }
    public void InitHP(int curHP, int maxHP)
    {
        _maxHP = maxHP;
        _currentHP = curHP;
        UpdateDamage();
    }
    public void DamageRoom(int damage)
    {
        _currentHP = Mathf.Max(0, _currentHP - damage);
        UpdateDamage();
    }
    public void UpdateDamage()
    {
        int hpLevel = Mathf.FloorToInt((_currentHP / (float)_maxHP) * 3);
        _floorRenderer.sprite = Resources.Load(GS.RoomGraphics(_floorType.ToString() + hpLevel.ToString()), typeof (Sprite)) as Sprite;
        if (_tGeo.GetComponent<PlayerTankController>()) _roomUI.roomUIObjects.SetActive(_currentHP < _maxHP);
        UpdateVehicleRoomHP();
    }

    public void ShowFloor(bool show)
    {
        _floorRenderer.gameObject.SetActive(show);
    }
    public void ShowRoof(bool show)
    {
        _roofRenderer.gameObject.SetActive(show);
    }
    public void ShowWalls(bool show)
    {
        foreach (RoomPosition pos in allRoomPositions)
        {
            if (pos._spawnedBottomWall) pos._spawnedBottomWall.SetActive(show);
            if (pos._spawnedLeftWall) pos._spawnedLeftWall.SetActive(show);
            if (pos._spawnedTopWall) pos._spawnedTopWall.SetActive(show);
            if (pos._spawnedRightWall) pos._spawnedRightWall.SetActive(show);
        }
    }
    public void ShowSystem(bool show)
    {
        foreach (RoomPosition pos in allRoomPositions)
        {
            if (pos._spawnedSystem) pos._spawnedSystem.gameObject.SetActive(show);
        }
    }
    public void ShowTire(bool show)
    {
        foreach (RoomPosition pos in allRoomPositions)
        {
            if (pos._spawnedTire) pos._spawnedTire.gameObject.SetActive(show);
        }
    }

    private void UpdateVehicleRoomHP()
    {
        foreach (RoomPosition roomPos in allRoomPositions)
        {
            _tGeo._vehicleData.VehicleMatrix.Columns[roomPos._xPos].ColumnContent[roomPos._yPos].RoomCurrentHP = _currentHP;
        }
    }

    private void InitRoomPositions()
    {
        freeRoomPositions = new RoomPosition[allRoomPositions.Length];
        for (int i = 0; i < allRoomPositions.Length; i++)
        {
            freeRoomPositions[i] = allRoomPositions[i];
            freeRoomPositions[i].roomPosIndex = i;
            freeRoomPositions[i].ParentRoom = this;
        }
    }
    public void OccupyRoomPos(RoomPosition rPos, AUnit unit)
    {
        freeRoomPositions[rPos.roomPosIndex] = null;
        if(!UnitsInRoom.Contains(unit)) UnitsInRoom.Add(unit);
    }
    public void FreeUpRoomPos(RoomPosition rPos, AUnit unit)
    {
        freeRoomPositions[rPos.roomPosIndex] = rPos;
        if (UnitsInRoom.Contains(unit)) UnitsInRoom.Remove(unit);

        //  if we have a system...
        if(_roomSystem != null)
        {
            //  check if the roomPosForInteraction is free
            if(freeRoomPositions[_roomSystem.RoomPosForInteraction.roomPosIndex])
            {
                //if we have more wizards in the room to fill up the space
                if (UnitsInRoom.Count > 0)
                { 
                    for(int i = 0; i < UnitsInRoom.Count-1; i++)
                    {
                        if (UnitsInRoom[i].UnitSelected) continue;
                        AUnit wizard = UnitsInRoom[i];

                        //free up the wizards current Pos
                        freeRoomPositions[wizard.CurrentRoomPos.roomPosIndex] = wizard.CurrentRoomPos;

                        //Assign where the wizard goes to
                        wizard.DesiredRoom = _roomSystem.RoomPosForInteraction.ParentRoom;
                        wizard.DesiredRoomPos = _roomSystem.RoomPosForInteraction;

                        //Occupy the roomPosInteraction spot again
                        OccupyRoomPos(_roomSystem.RoomPosForInteraction, wizard);

                        //Assign the valid path to the roomPos
                        wizard.ClearUnitPath();
                        wizard.CurrentWaypoint = 0;
                        wizard.PathToRoom = REF.Path.FindPath(wizard.CurrentRoomPos, _roomSystem.RoomPosForInteraction, wizard.transform.parent.GetComponentInChildren<TankGeometry>());

                        //start the movement

                        wizard._unitState = UnitState.Moving;
                        wizard.UnitSelected = false;

                        //Set the indicator of the unit
                        unit.SetNextPosIndicator(unit.DesiredRoomPos);

                        break; //make sure we only do this for the first wizard and not multiple
                    }
                }
            }
        }
    }

    public RoomPosition GetNextFreeRoomPos()
    {
        for(int i = 0; i < freeRoomPositions.Length; i++)
        {
            if (freeRoomPositions[i]) return freeRoomPositions[i];
        }
        return null;
    }
    public List<RoomPosition> GetAllFreeRoomPos()
    {
        List<RoomPosition> freePos = new List<RoomPosition>();
        for (int i = 0; i < freeRoomPositions.Length; i++)
        {
            if (freeRoomPositions[i]) freePos.Add(freeRoomPositions[i]);
        }
        return freePos;
    }
}