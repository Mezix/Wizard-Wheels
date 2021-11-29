using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public TankGeometry tGeo;
    public TankRoomConstellation tr;
    public ISystem roomSystem;
    public SpriteRenderer roomSystemRenderer;
    public int sizeX;
    public int sizeY;
    
    public RoomPosition[] allRoomPositions;
    public RoomPosition[] freeRoomPositions;

    public List<AUnit> WizardsInRoom = new List<AUnit>();
    private void Awake()
    {
        InitRoomPositions();
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
        if(!WizardsInRoom.Contains(unit)) WizardsInRoom.Add(unit);
    }
    public void FreeUpRoomPos(RoomPosition rPos, AUnit unit)
    {
        freeRoomPositions[rPos.roomPosIndex] = rPos;
        if (WizardsInRoom.Contains(unit)) WizardsInRoom.Remove(unit);

        //  if we have a system...
        if(roomSystem != null)
        {
            //  check if the roomPosForInteraction is free
            if(freeRoomPositions[roomSystem.RoomPosForInteraction.roomPosIndex])
            {
                //if we have more wizards in the room to fill up the space
                if (WizardsInRoom.Count > 0)
                { 
                    for(int i = 0; i < WizardsInRoom.Count-1; i++)
                    {
                        if (WizardsInRoom[i].UnitSelected) continue;
                        AUnit wizard = WizardsInRoom[i];

                        //free up the wizards current Pos
                        freeRoomPositions[wizard.CurrentRoomPos.roomPosIndex] = wizard.CurrentRoomPos;

                        //Assign where the wizard goes to
                        wizard.DesiredRoom = roomSystem.RoomPosForInteraction.ParentRoom;
                        wizard.DesiredRoomPos = roomSystem.RoomPosForInteraction;

                        //Occupy the roomPosInteraction spot again
                        OccupyRoomPos(roomSystem.RoomPosForInteraction, wizard);

                        //Assign the valid path to the roomPos
                        wizard.ClearUnitPath();
                        wizard.CurrentWaypoint = 0;
                        wizard.PathToRoom = Ref.Path.FindPath(wizard.CurrentRoomPos, roomSystem.RoomPosForInteraction, wizard.transform.parent.GetComponentInChildren<TankGeometry>());

                        //start the movement
                        wizard.UnitIsMoving = true;
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
            //try get the next free room
        }
        return null;
    }
}