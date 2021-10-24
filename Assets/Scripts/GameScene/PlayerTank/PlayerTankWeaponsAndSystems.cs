using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankWeaponsAndSystems : MonoBehaviour
{
    public List<IWeapon> IWeaponArray = new List<IWeapon>();
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
        if (weaponIndex < IWeaponArray.Count) IWeaponArray[weaponIndex].WeaponSelected = true;
    }
    public void DeselectAllWeapons()
    {
        foreach (IWeapon wp in IWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
        }
    }
    public void InitWeapons()
    {
        //foreach (IWeapon wp in GetComponentsInChildren<IWeapon>())
        //{
        //    IWeaponArray.Add(wp);
        //    wp.ShouldHitPlayer = false;
        //    AddWeaponSystemToNearestRoom(wp.WeaponObj);
        //}

        TankRoomConstellation tr = Ref.PCon.TGeo._tankRoomConstellation;
        for (int x = 0; x < tr.XTilesAmount; x++)
        {
            for (int y = 0; y < tr.YTilesAmount; y++)
            {
                if (tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab)
                {
                    GameObject prefab = tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab;
                    //Our object should either be a Weapon or a system, so check for both cases
                    if (tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<IWeapon>() != null)
                    {
                        GameObject weaponObj = Instantiate(prefab);
                        IWeapon wep = weaponObj.GetComponent<IWeapon>();
                        IWeaponArray.Add(wep);
                        wep.ShouldHitPlayer = false;
                    }
                    else if(tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<ISystem>() != null)
                    {
                        GameObject systemObj = Instantiate(prefab);
                        ISystem sys = systemObj.GetComponent<IWeapon>();
                        ISystemArray.Add(sys);
                    }
                }
            }
        }
    }

    private void AddWeaponSystemToNearestRoom(GameObject weapon)
    {
        Room r = Ref.PCon.TGeo.FindRandomRoomWithSpace();
        r.roomSystem = weapon.GetComponent<ISystem>();
        r.roomSystemRenderer.sprite = weapon.GetComponent<IWeapon>().WeaponSprite;
    }

    public void ClearWeapons()
    {
        IWeaponArray.Clear();
    }
    public void CreateWeaponsUI()
    {
        for (int i = 0; i < IWeaponArray.Count; i++)
        {
            IWeaponArray[i].SetIndex(i+1);
            UIWeapon uw = Ref.UI.CreateWeaponUI(IWeaponArray[i]);
            AllUIWeapons.Add(uw);
            IWeaponArray[i].UIWep = uw;
        }
    }
    public void WeaponBehaviourInDeath()
    {
        DeselectAllWeapons();
        foreach (IWeapon wp in IWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
            wp.ShouldNotRotate = true;
            Ref.c.RemoveCrosshair(wp);
        }
    }
}
