using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    //Buttons
    public Button CruiseButton;
    public Button RotateBackButton;
    public Button MatchSpeedButton;
    public Button PauseButton;
    public Button SettingsButton;
    public Button TrackPlayerTankButton;

    //Sliders
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

    // Health
    public GameObject _healthBarParent;
    public List<Image> _allHealthBarUnits;
    public GameObject _healthBarUnitPrefab;

    private void Awake()
    {
        settingsOn = false;
        PauseImage.SetActive(false);
        Ref.UI = this;
    }
    private void Start()
    {
        InitButtons();
        InitSliders();
    }
    private void InitButtons()
    {
        CruiseButton.onClick = new Button.ButtonClickedEvent();
        CruiseButton.onClick.AddListener(() => Ref.PCon.TMov.ToggleCruise());
        MatchSpeedButton.onClick = new Button.ButtonClickedEvent();
        MatchSpeedButton.onClick.AddListener(() => MatchSpeed());
        RotateBackButton.onClick = new Button.ButtonClickedEvent();
        RotateBackButton.onClick.AddListener(() => Ref.PCon.TRot.TurnTankUp());
        SettingsButton.onClick = new Button.ButtonClickedEvent();
        SettingsButton.onClick.AddListener(() => ToggleSettings());
        PauseButton.onClick = new Button.ButtonClickedEvent();
        PauseButton.onClick.AddListener(() => Ref.TM.TogglePauseWhilstPlaying());
        TrackPlayerTankButton.onClick = new Button.ButtonClickedEvent();
        TrackPlayerTankButton.onClick.AddListener(() => Ref.Cam.SetTrackedVehicleToPlayer());
    }
    private void InitSliders()
    {
        _currentSpeedSlider.value = Ref.PCon.TMov.velocity;
        _desiredSpeedSlider.value = Ref.PCon.TMov.velocity;
        _currentSpeedSlider.maxValue = Ref.PCon.TMov.maxVelocity;
        _desiredSpeedSlider.maxValue = Ref.PCon.TMov.maxVelocity;
    }
    public void ToggleSettings()
    {
        if (!settingsOn) OpenSettings();
        else CloseSettings();
    }
    public void OpenSettings()
    {
        Ref.TM.FreezeTime();
        Settings.SetActive(true);
        settingsOn = true;
    }
    public void CloseSettings()
    {
        if(!TimeManager.paused) Ref.TM.UnfreezeTime();
        Settings.SetActive(false);
        settingsOn = false;
    }
    public UIWeapon CreateWeaponUI(IWeapon iwp)
    {
        if (!weaponsUIPrefab)
        {
            Debug.LogWarning("Weapon UI Prefab not assigned, wont spawn UI");
            return null;
        }
        GameObject go = Instantiate((GameObject) Resources.Load("Weapons\\UI Weapon"));
        UIWeapon wp = go.GetComponent<UIWeapon>();
        wp._weaponImage.sprite = iwp.WeaponSprite;
        wp._UIWeaponName.text = iwp.WeaponName;
        wp._index = iwp.WeaponIndex;
        wp._weapon = iwp;
        wp._UIWeaponIndex.text = iwp.WeaponIndex.ToString();

        go.transform.SetParent(weaponsList.transform,false);
        return wp;
    }
    public void SpeedSliderUpdated()
    {
        PlayerTankController.instance.TMov.cruiseModeOn = true;
    }
    public void SteeringWheelPointerUpdated()
    {

    }

    public void TurnOnCruiseMode(bool b)
    {
        if (b) CruiseButton.image.color = Color.black;
        else CruiseButton.image.color = Color.white;
    }
    private void MatchSpeed()
    {
        print("implement this!");
    }
    public void CreateHealthbar(int maxHealth)
    {
        if(_allHealthBarUnits.Count > 0)
        {
            foreach(Image g in _allHealthBarUnits) Destroy(g);
            _allHealthBarUnits.Clear();
        }
        for(int i = 0; i < maxHealth; i++)
        {
            GameObject tmp = Instantiate(_healthBarUnitPrefab);
            _allHealthBarUnits.Add(tmp.GetComponent<Image>());
            tmp.transform.SetParent(_healthBarParent.transform, false);
        }
    }
    public void UpdateHealthBar(int current, int maxHealth)
    {
        current = Mathf.Max(0, current);
        for (int i = 0; i < maxHealth - 1; i++)
        {
            SetHealthUnitStatus(i, true); //set all health to full
        }
        for (int i = maxHealth-1; i > current-1; i--)
        {
            SetHealthUnitStatus(i, false); //now set all the destroyed health
        }
    }
    public void SetHealthUnitStatus(int i, bool full)
    {
        if(full) _allHealthBarUnits[i].color = Color.white;
        else _allHealthBarUnits[i].color = Color.black;
    }
    public void SpawnGameOverScreen()
    {
        GameObject gameOver = Instantiate((GameObject) Resources.Load("GameOverScreen"));
        gameOver.transform.position = Vector3.zero;
        gameOver.transform.SetParent(transform, false);
        List<Button> buttons =  gameOver.GetComponentsInChildren<Button>().ToList();
        foreach(Button b in buttons)
        {
            b.onClick = new Button.ButtonClickedEvent();
            b.onClick.AddListener(() => LevelManager.instance.GoToMainMenu());
        }
    }
}
