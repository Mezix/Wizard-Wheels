using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;
    public static bool paused;

    public Button CruiseButton;
    public Button RotateBackButton;
    public Slider _currentSpeedSlider;
    public Slider _desiredSpeedSlider;

    public GameObject PauseImage;
    public GameObject SteeringWheel;
    public GameObject SteeringWheelPointer;

    public GameObject weaponsUIPrefab;
    public GameObject weaponsList;

    private void Awake()
    {
        paused = false;
        PauseImage.SetActive(false);
        instance = this;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ToggleTime();
        }
    }
    public void ToggleTime()
    {
        paused = !paused;
        if (paused) Time.timeScale = 0;
        else Time.timeScale = 1;

        PauseImage.SetActive(paused);
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
