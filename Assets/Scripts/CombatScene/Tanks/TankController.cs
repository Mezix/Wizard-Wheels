using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;

public abstract class TankController : MonoBehaviour
{
    public TankStats _tStats;
    public TankRotation TRot { get; set; }
    public TankGeometry TGeo { get; set; }
    public TankHealth THealth { get; set; }

    [HideInInspector]
    public string _tankName;

    //  Wizards
    //public List<GameObject> _wizardsToSpawn = new List<GameObject>();
    public List<WizardData> _wizardData = new List<WizardData>();
    public List<AUnit> _spawnedWizards = new List<AUnit>();

    public bool _dying;
    public bool _dead;

    public Color _tankColor;

    public void SpawnWizards()
    {
        int wizardIndex = 0;
        foreach (WizardData wiz in _wizardData)
        {
            AUnit unit = Instantiate(Resources.Load(GS.Wizards(wiz.WizType.ToString()), typeof (AUnit)) as AUnit);
            Vector2Int roomPosVector = new Vector2Int(0,0);
            if (wiz.RoomPositionX == -1 || wiz.RoomPositionY == -1)
            {
                roomPosVector = TGeo.FindRoomPositionWithSpace();
                DataStorage.Singleton.playerData.WizardList[wizardIndex].RoomPositionX = roomPosVector.x;
                DataStorage.Singleton.playerData.WizardList[wizardIndex].RoomPositionY = roomPosVector.y;
            }
            else
            {
                roomPosVector = new Vector2Int(wiz.RoomPositionX, wiz.RoomPositionY);
            }
            RoomPosition roomPosToGoTo = TGeo.RoomPosMatrix[roomPosVector.x, roomPosVector.y];
            unit.transform.parent = transform;
            unit.transform.position = roomPosToGoTo.transform.position;
            unit.CurrentRoom = roomPosToGoTo.ParentRoom;
            unit.CurrentRoom.OccupyRoomPos(roomPosToGoTo, unit);
            unit.CurrentRoomPos = roomPosToGoTo;
            unit.Index = wizardIndex;
            _spawnedWizards.Add(unit);

            unit.InitUnit();
            wizardIndex++;
        }
    }
    public virtual void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
}
