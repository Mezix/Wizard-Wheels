using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;

    public Button CruiseButton;
    public Button RotateBackButton;
    public Slider _currentSpeedSlider;
    public Slider _desiredSpeedSlider;

    public GameObject PauseImage;
    public GameObject SteeringWheel;
    public GameObject SteeringWheelPointer;

    // Weapons
    public GameObject weaponsUIPrefab;
    public GameObject weaponsList;

    // Settings
    public GameObject Settings;
    public bool settingsOn;

    private void Awake()
    {
        settingsOn = false;
        PauseImage.SetActive(false);
        instance = this;
    }
    private void Update()
    {
    }
    
    public void ToggleSettings()
    {
        if (!settingsOn) OpenSettings();
        else CloseSettings();
    }
    public void OpenSettings()
    {
        TimeManager.instance.FreezeTime();
        Settings.SetActive(true);
        settingsOn = true;
    }
    public void CloseSettings()
    {
        if(!TimeManager.paused) TimeManager.instance.UnfreezeTime();
        Settings.SetActive(false);
        settingsOn = false;
    }
    public void CreateWeaponUI(int idx, IWeapon iwp, Image wpImg = null, string wpName = null)
    {
        if (!weaponsUIPrefab)
        {
            Debug.LogWarning("Weapon UI Prefab not assigned, wont spawn UI");
            return;
        }
        GameObject go = Instantiate(weaponsUIPrefab);
        UIWeapon wp = go.GetComponent<UIWeapon>();
        //wp.weaponImage = img;
        //wp.UIWeaponName = wpName;
        wp.index = idx;
        wp.weapon = iwp;
        wp.UIWeaponIndex.text = idx.ToString();

        go.transform.SetParent(weaponsList.transform,false);
    }
    public void SpeedSliderUpdated()
    {
        PlayerTankController.instance.tMov.cruiseModeOn = true;
    }
    public void SteeringWheelPointerUpdated()
    {

    }

    public void TurnOnCruiseMode(bool b)
    {
        if (b) CruiseButton.image.color = Color.black;
        else CruiseButton.image.color = Color.white;
    }
}
