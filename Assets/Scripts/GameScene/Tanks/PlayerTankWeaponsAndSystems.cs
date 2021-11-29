using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankWeaponsAndSystems : MonoBehaviour
{
    public List<AWeapon> AWeaponArray = new List<AWeapon>();
    public List<ISystem> ISystemArray = new List<ISystem>();
    public List<UIWeapon> AllUIWeapons = new List<UIWeapon>();
    public bool multipleSelected = true;

    private void Update()
    {
        if (!Ref.PCon._dying)
        {
            if (Input.GetKey(KeyCode.LeftShift)) multipleSelected = true;
            else multipleSelected = false;
            HandleWeaponSelection();
        }
    }

    private void HandleWeaponSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectWeapon(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectWeapon(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectWeapon(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectWeapon(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SelectWeapon(8);
    }

    private void SelectWeapon(int weaponIndex)
    {
        if (!multipleSelected) DeselectAllWeapons();
        if (weaponIndex < AWeaponArray.Count) AWeaponArray[weaponIndex].WeaponSelected = true;
    }
    public void DeselectAllWeapons()
    {
        foreach (AWeapon wp in AWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
        }
    }
    public void InitWeaponsAndSystems()
    {
        TankGeometry tGeo = GetComponent<TankGeometry>();
        for (int x = 0; x < tGeo._tankRoomConstellation.XTilesAmount; x++)
        {
            for (int y = 0; y < tGeo._tankRoomConstellation.YTilesAmount; y++)
            {
                if (tGeo._tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab)
                {
                    GameObject prefab = tGeo._tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab;

                    //Our object should either be a Weapon or a System, so check for both cases
                    if (tGeo._tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<AWeapon>() != null)
                    {
                        if (!tGeo.RoomPosMatrix[x, y]) continue;
                        GameObject weaponObj = Instantiate(prefab);
                        weaponObj.transform.parent = tGeo.RoomPosMatrix[x,y].ParentRoom.transform;
                        weaponObj.transform.localPosition = Vector3.zero;
                        AWeapon wep = weaponObj.GetComponent<AWeapon>();
                        wep.InitSystem();
                        PositionSystemInRoom(weaponObj.GetComponent<ISystem>(), weaponObj.transform.parent.GetComponent<Room>());
                        wep.ShouldHitPlayer = false;
                        wep.EnemyWepUI.ShowWeaponUI(false);
                        wep.RoomPosForInteraction = tGeo.RoomPosMatrix[x, y].ParentRoom.allRoomPositions[0];
                        AWeaponArray.Add(wep);

                        //Set the reference to the rooms
                        tGeo.RoomPosMatrix[x, y].ParentRoom.roomSystem = wep;
                        tGeo.RoomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = wep.SystemSprite;
                    }
                    else if(tGeo._tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<ISystem>() != null)
                    {
                        GameObject systemObj = Instantiate(prefab);
                        systemObj.transform.parent = tGeo.RoomPosMatrix[x, y].ParentRoom.transform;

                        systemObj.transform.localPosition = Vector3.zero;
                        ISystem sys = systemObj.GetComponent<ISystem>();
                        sys.InitSystem();
                        PositionSystemInRoom(systemObj.GetComponent<ISystem>(), systemObj.transform.parent.GetComponent<Room>());
                        ISystemArray.Add(sys);
                        sys.RoomPosForInteraction = tGeo.RoomPosMatrix[x, y].ParentRoom.allRoomPositions[0];

                        //Set the reference to the rooms
                        tGeo.RoomPosMatrix[x, y].ParentRoom.roomSystem = sys;
                        tGeo.RoomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = sys.SystemSprite;
                    }
                }
            }
        }
    }

    private void PositionSystemInRoom(ISystem system, Room parentRoom)
    {
        system.SystemObj.transform.localPosition = Vector2.zero;
        if (parentRoom.sizeX > 1) system.SystemObj.transform.localPosition += new Vector3(0.25f, 0);
        if (parentRoom.sizeY > 1) system.SystemObj.transform.localPosition += new Vector3(0, -0.25f);
    }

    private void AddWeaponSystemToNearestRoom(GameObject weapon)
    {
        Room r = Ref.PCon.TGeo.FindRandomRoomWithSpace();
        r.roomSystem = weapon.GetComponent<ISystem>();
        r.roomSystemRenderer.sprite = weapon.GetComponent<AWeapon>().SystemSprite;
    }

    public void ClearWeapons()
    {
        AWeaponArray.Clear();
    }
    public void CreateWeaponsUI()
    {
        for (int i = 0; i < AWeaponArray.Count; i++)
        {
            AWeaponArray[i].SetIndex(i+1);
            UIWeapon uw = Ref.UI.CreateWeaponUI(AWeaponArray[i]);
            AllUIWeapons.Add(uw);
            AWeaponArray[i].PlayerUIWep = uw;
        }
    }
    public void WeaponBehaviourInDeath()
    {
        DeselectAllWeapons();
        foreach (AWeapon wp in AWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
            wp.ShouldNotRotate = true;
            Ref.c.RemoveCrosshair(wp);
        }
    }
}
