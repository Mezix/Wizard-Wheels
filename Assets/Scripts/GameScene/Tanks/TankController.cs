using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankController : MonoBehaviour
{
    public TankStats _tStats;
    public TankRotation TRot { get; set; }
    public TankGeometry TGeo { get; set; }
    public TankHealth THealth { get; set; }
    [HideInInspector]
    public string _tankName;

    //  Wizards
    public List<GameObject> _wizardsToSpawn = new List<GameObject>();
    public List<AUnit> _spawnedWizards = new List<AUnit>();

    public bool _dying;
    public bool _dead;

    public void SpawnWizards()
    {
        int wizardIndex = 1;
        foreach (GameObject wiz in _wizardsToSpawn)
        {
            GameObject wizGO = Instantiate(wiz);
            AUnit u = wizGO.GetComponentInChildren<AUnit>();
            Room room = TGeo.FindRandomRoomWithSpace();
            wizGO.transform.parent = transform;
            wizGO.transform.position = room.transform.position;
            u.CurrentRoom = room;
            u.CurrentRoom.OccupyRoomPos(room.GetNextFreeRoomPos(), u);
            u.CurrentRoomPos = u.CurrentRoom.allRoomPositions[0];
            u.Index = wizardIndex;
            _spawnedWizards.Add(wizGO.GetComponentInChildren<AUnit>());

            u.InitUnit();
            wizardIndex++;
        }
    }
    public void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
}
