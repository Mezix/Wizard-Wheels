using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    string UnitName { get; set; }
    string UnitClass { get; set; }
    float UnitHealth { get; set; }
    float UnitSpeed { get; set; }
    SpriteRenderer Rend { get; set; }
    bool UnitSelected { get; set; }
    bool UnitIsMoving { get; set; }
    GameObject UnitObj { get; set; }
    UIWizard UIWizard { get; set; }

    Room CurrentRoom { get; set; }
    RoomPosition CurrentRoomPos { get; set; }
    Room DesiredRoom { get; set; }
    RoomPosition DesiredRoomPos { get; set; }
    List<RoomPosition> PathToRoom { get; set; }
    int currentWaypoint{ get; set; }

    void InitUnit();
    void ClearPathToRoom();
    void StopInteraction();
}
