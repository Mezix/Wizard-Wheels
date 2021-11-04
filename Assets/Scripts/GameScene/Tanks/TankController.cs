using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankController : MonoBehaviour
{
    public TankStats _tStats;
    public TankRotation TRot { get; set; }
    public TankGeometry TGeo { get; set; }

    public string _tankName;

    //  Wizards
    public List<GameObject> _wizardsToSpawn = new List<GameObject>();
    public List<IUnit> _spawnedWizards = new List<IUnit>();

    public bool _dying;
    public bool _dead;
    public void SpawnWizards()
    {
        foreach (GameObject wiz in _wizardsToSpawn)
        {
            GameObject wizGO = Instantiate(wiz);
            IUnit u = wizGO.GetComponentInChildren<IUnit>();
            Room room = TGeo.FindRandomRoomWithSpace();
            wizGO.transform.parent = transform;
            wizGO.transform.position = room.transform.position;
            u.CurrentRoom = room;
            u.CurrentRoom.OccupyRoomPos(room.GetNextFreeRoomPos(), u);
            u.CurrentRoomPos = u.CurrentRoom.allRoomPositions[0];
            _spawnedWizards.Add(wizGO.GetComponentInChildren<IUnit>());

            u.InitUnit();
        }
    }

}
