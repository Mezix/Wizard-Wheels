using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    //Buttons
    public Button _cruiseButton;
    public Button _rotateBackButton;
    public Button _matchSpeedButton;
    public Button _pauseButton;
    public Button _xrayButton;
    public Button _settingsButton;
    public Button _trackPlayerTankButton;
    public Button _returnWizardsButton;
    public Button _saveWizardsButton;

    //Sliders
    public Slider _currentSpeedSlider;
    public Slider _desiredSpeedSlider;

    public GameObject _pauseImage;
    public GameObject _steeringWheel;
    public GameObject _steeringWheelPointer;

    // Weapons
    public GameObject _weaponsList;
    public GameObject _wizardsList;

    // Settings
    public GameObject _settings;
    public bool _settingsOn;

    // Health
    public GameObject _healthBarParent;
    public List<Image> _allHealthBarUnits;

    //  Vision 
    public bool _xrayOn;
    public Image _xrayImage;

    //  MatchSpeed
    private bool matchingSpeed;

    private void Awake()
    {
        _settingsOn = false;
        Ref.UI = this;
    }
    private void Start()
    {
        InitButtons();
        InitSliders();
        _xrayOn = true;
        _pauseImage.SetActive(false);
        CloseSettings();
    }
    private void InitButtons()
    {
        _cruiseButton.onClick = new Button.ButtonClickedEvent();
        _cruiseButton.onClick.AddListener(() => Ref.PCon.TMov.ToggleCruise());
        _matchSpeedButton.onClick = new Button.ButtonClickedEvent();
        _matchSpeedButton.onClick.AddListener(() => StartToMatchSpeed());
        _rotateBackButton.onClick = new Button.ButtonClickedEvent();
        _rotateBackButton.onClick.AddListener(() => Ref.PCon.TRot.GetComponent<PlayerTankRotation>().TurnTankUp());
        _settingsButton.onClick = new Button.ButtonClickedEvent();
        _settingsButton.onClick.AddListener(() => ToggleSettings());
        _pauseButton.onClick = new Button.ButtonClickedEvent();
        _pauseButton.onClick.AddListener(() => Ref.TM.TogglePauseWhilstPlaying());
        _xrayButton.onClick = new Button.ButtonClickedEvent();
        _xrayButton.onClick.AddListener(() => ToggleVision());
        _trackPlayerTankButton.onClick = new Button.ButtonClickedEvent();
        _trackPlayerTankButton.onClick.AddListener(() => Ref.Cam.SetTrackedVehicleToPlayer());
        _returnWizardsButton.onClick = new Button.ButtonClickedEvent();
        _returnWizardsButton.onClick.AddListener(() => Ref.PCon.ReturnAllWizardsToSavedPositions());
        _saveWizardsButton.onClick = new Button.ButtonClickedEvent();
        _saveWizardsButton.onClick.AddListener(() => Ref.PCon.SaveAllWizardPositions());
    }

    

    private void InitSliders()
    {
        _currentSpeedSlider.value = Ref.PCon.TMov.currentSpeed;
        _desiredSpeedSlider.value = Ref.PCon.TMov.currentSpeed;
        _currentSpeedSlider.maxValue = Ref.PCon.TMov.maxSpeed;
        _desiredSpeedSlider.maxValue = Ref.PCon.TMov.maxSpeed;
    }
    public void ToggleSettings()
    {
        if (!_settingsOn) OpenSettings();
        else CloseSettings();
    }
    public void OpenSettings()
    {
        Ref.TM.FreezeTime();
        _settings.SetActive(true);
        _settingsOn = true;
    }
    public void CloseSettings()
    {
        if(!TimeManager.paused) Ref.TM.UnfreezeTime();
        _settings.SetActive(false);
        _settingsOn = false;
    }
    public UIWeapon CreateWeaponUI(AWeapon iwp)
    {
        GameObject go = Instantiate((GameObject) Resources.Load("Weapons\\UIWeapon"));
        UIWeapon wp = go.GetComponent<UIWeapon>();
        wp._weaponImage.sprite = iwp.SystemSprite;
        wp._UIWeaponName.text = iwp.SystemName;
        wp._index = iwp.EnemyWepUI.WeaponIndex;
        wp._weapon = iwp;
        wp._UIWeaponIndex.text = iwp.EnemyWepUI.WeaponIndex.ToString();

        go.transform.SetParent(_weaponsList.transform,false);
        return wp;
    }
    public UIWizard CreateWizardUI(AUnit unit)
    {
        GameObject go = Instantiate((GameObject)Resources.Load("UIWizard"));
        UIWizard u = go.GetComponent<UIWizard>();
        u._wizardImage.sprite = unit.Rend.sprite;
        u._UIWizardName.text = unit.UnitName;
        u._index = unit.Index;
        u._UIWizardKeybind.text = "CTRL + " + u._index.ToString();
        u.button.onClick = new Button.ButtonClickedEvent();
        u.button.onClick.AddListener(() => u.SelectWizard());
        u.wizard = unit;

        go.transform.SetParent(_wizardsList.transform, false);
        return u;
    }

    public void SpeedSliderUpdated()
    {
        Ref.PCon.TMov.cruiseModeOn = true;
    }
    public void TurnOnCruiseMode(bool b)
    {
        if (b) _cruiseButton.image.color = Color.black;
        else _cruiseButton.image.color = Color.white;
    }
    private void StartToMatchSpeed()
    {
        if(Ref.PCon)
        {
            Ref.PCon.GetComponent<PlayerTankMovement>()._attemptingMatchingSpeed = true;
            Ref.PCon.GetComponent<PlayerTankMovement>().enemyToMatch = null;
            Ref.PCon.GetComponent<PlayerTankMovement>()._matchSpeed = false;
        }
    }

    //  XRay

    private void ToggleVision()
    {
        TurnOnXRay(!_xrayOn);
        _xrayOn = !_xrayOn;
    }
    public void TurnOnXRay(bool xrayOn)
    {
        if (xrayOn)
        {
            _xrayImage.sprite = Resources.Load("Art\\eye_opened", typeof(Sprite)) as Sprite;

            //  Player

            if(Ref.PCon)
            {
                foreach(AWeapon wep in Ref.PCon.TWep.AWeaponArray)
                {
                    wep.SetOpacity(true);
                }
                Ref.PCon.TGeo.ShowRoof(false);
            }

            //  Enemies

            if(Ref.EM)
            {
                if(Ref.EM._enemies.Count > 0)
                {
                    foreach(GameObject g in Ref.EM._enemies)
                    {
                        foreach(AWeapon wep in g.GetComponent<EnemyTankController>().TWep.AWeaponArray)
                        {
                             wep.SetOpacity(true);
                        }
                        g.GetComponent<EnemyTankController>().TGeo.ShowRoof(false);
                    }
                }
            }

            //  Toggle Icons to be underneath roof

        }
        else
        {
            _xrayImage.sprite = Resources.Load("Art\\eye_closed", typeof(Sprite)) as Sprite;

            //  Player

            if (Ref.PCon)
            {
                foreach (AWeapon wep in Ref.PCon.TWep.AWeaponArray)
                {
                    wep.SetOpacity(false);
                    Ref.PCon.TGeo.ShowRoof(true);
                }
            }
            
            //  Enemies

            if (Ref.EM)
            {
                if (Ref.EM._enemies.Count > 0)
                {
                    foreach (GameObject g in Ref.EM._enemies)
                    {
                        foreach (AWeapon wep in g.GetComponent<EnemyTankController>().TWep.AWeaponArray)
                        {
                            wep.SetOpacity(false);
                        }
                        g.GetComponent<EnemyTankController>().TGeo.ShowRoof(true);
                    }
                }
            }

            //  Toggle Icons to be display over roof layer
        }
    }

    //  Health
    public void CreateHealthbar(int maxHealth)
    {
        if(_allHealthBarUnits.Count > 0)
        {
            foreach(Image g in _allHealthBarUnits) Destroy(g);
            _allHealthBarUnits.Clear();
        }
        for(int i = 0; i < maxHealth; i++)
        {
            GameObject tmp = Instantiate((GameObject) Resources.Load("HPSegment"));
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
