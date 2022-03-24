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
    public Button _pauseButton;
    public Button _xrayButton;
    public Button _settingsButton;
    public Button _trackPlayerTankButton;

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
    public List<Image> _allHPSegments;

    //  XRay 
    public bool _xrayOn;
    public Image _xrayImage;

    //  MatchSpeed
    public Image _matchSpeedImage;
    public Button _unmatchSpeedButton;

    //  Double Clicks
    public float timeBetweenMouseClicks;
    public GameObject LastWizardOrWeaponClicked;

    //  Weapon Out Of Range
    public GameObject WeaponOutOfRangeParent;

    //  Save and return Wizards to Positions
    public Button _saveWizardsButton;
    public Image _saveWizardsImage;
    public Button _returnWizardsButton;
    public Image _returnWizardsImage;
    private bool wizardsSaved;

    //  Emergency Brake
    public Toggle EmergencyBrakeToggle;

    //  Engine Level
    public Transform _engineLevel;
    private List<Image> engineLevelSegments;

    //  Upgrade Screen
    public UpgradeScreen _upgradeScreen;

    //  Steering Wheel
    public RectTransform _steeringWheelObject;
    public Transform steeringWheelParent;
    public Button _rotateBackButton;
    public GameObject _steeringWheelPrompt;
    private bool steeringWheelOpen;
    private float minHoldTime = 0.75f;
    private float holdTime = 0;

    private void Awake()
    {
        _settingsOn = false;
        wizardsSaved = false;
        Ref.UI = this;
        steeringWheelOpen = false;
    }
    private void Start()
    {
        SaveWizards(wizardsSaved);
        timeBetweenMouseClicks = 0;
        InitButtons();
        InitEngineLevel();
        _xrayOn = true;
        _pauseImage.SetActive(false);
        CloseSettings();
        Events.instance.WizardOrWeaponClicked += CheckDoubleClick;
    }

    private void CheckDoubleClick(GameObject obj)
    {
        if (LastWizardOrWeaponClicked)
            if (LastWizardOrWeaponClicked.Equals(obj))
                if (timeBetweenMouseClicks < 0.25f)
                    Ref.Cam.SetTrackedVehicleToObject(obj.transform);
        Ref.Cam.SetDesiredZoom(Ref.Cam.minZoom);

        LastWizardOrWeaponClicked = obj;
        timeBetweenMouseClicks = 0;
    }

    private void Update()
    {
        timeBetweenMouseClicks += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleVision();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleSteeringWheel();
        }
        if(Input.GetKey(KeyCode.R))
        {
            holdTime += Time.deltaTime;
        }
        else
        {
            holdTime = 0;
            ResetSteeringWheel();
            _rotateBackButton.gameObject.SetActive(true);
        }

        if(holdTime >= minHoldTime)
        {
            SteeringWheelTrackMouse();
        }
        else
        {
            ResetSteeringWheel();
        }
    }

    private void ResetSteeringWheel()
    {
        if (steeringWheelOpen) OpenSteeringWheel();
        else CloseSteeringWheel();
    }

    private void InitButtons()
    {
        _cruiseButton.onClick = new Button.ButtonClickedEvent();
        _cruiseButton.onClick.AddListener(() => Ref.PCon.TMov.ToggleCruise());
        _unmatchSpeedButton.onClick.AddListener(() => UnmatchSpeedUI());
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
        _saveWizardsButton.onClick.AddListener(() => SaveWizards(true));
        EmergencyBrakeToggle.onValueChanged = new Toggle.ToggleEvent();
        EmergencyBrakeToggle.onValueChanged.AddListener(delegate {EmergencyBrake(EmergencyBrakeToggle);});
    }

    public void UnmatchSpeedUI()
    {
        _unmatchSpeedButton.gameObject.SetActive(false);
        Ref.PCon.TMov.enemyToMatch.GetComponent<EnemyTankController>().enemyUI.MatchSpeed(Ref.PCon.TMov.enemyToMatch, false);
    }

    public void InitSliders()
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
        if (!TimeManager.paused) Ref.TM.UnfreezeTime();
        _settings.SetActive(false);
        _settingsOn = false;
    }
    public PlayerWeaponUI CreateWeaponUI(AWeapon iwp)
    {
        GameObject go = Instantiate((GameObject)Resources.Load("Weapons\\PlayerWeaponUI"));
        PlayerWeaponUI wp = go.GetComponent<PlayerWeaponUI>();
        wp._weaponImage.sprite = iwp._weaponStats._UISprite;
        wp._UIWeaponName.text = iwp.SystemName;
        wp._index = iwp.EnemyWepUI.WeaponIndex;
        wp._weapon = iwp;
        wp._UIWeaponIndex.text = iwp.EnemyWepUI.WeaponIndex.ToString();

        go.transform.SetParent(_weaponsList.transform, false);
        return wp;
    }
    public PlayerWizardUI CreateWizardUI(AUnit unit)
    {
        GameObject go = Instantiate((GameObject)Resources.Load("Wizards\\PlayerWizardUI"));
        PlayerWizardUI u = go.GetComponent<PlayerWizardUI>();
        u._wizardImage.sprite = unit.PlayerUIWizardIcon;
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
        if(EmergencyBrakeToggle.isOn) Ref.UI.ActivateEmergencyBrake(false);
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
            _xrayImage.sprite = Resources.Load("Art\\UI\\XRayOn", typeof(Sprite)) as Sprite;

            //  Player

            if (Ref.PCon)
            {
                foreach (AWeapon wep in Ref.PCon.TWep.AWeaponArray)
                {
                    wep.SetOpacity(true);
                }
                Ref.PCon.TGeo.ShowRoof(false);
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
            _xrayImage.sprite = Resources.Load("Art\\UI\\XRayOff", typeof(Sprite)) as Sprite;

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
        if (_allHPSegments.Count > 0)
        {
            foreach (Image g in _allHPSegments) Destroy(g);
            _allHPSegments.Clear();
        }
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject tmp = Instantiate((GameObject)Resources.Load("HPSegment"));
            Image img = tmp.GetComponent<Image>();

            if (i == 0) img.sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Left", typeof(Sprite)) as Sprite;
            else if (i == maxHealth - 1) img.sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Right", typeof(Sprite)) as Sprite;
            else img.sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Middle", typeof(Sprite)) as Sprite;

            _allHPSegments.Add(img);
            tmp.transform.SetParent(_healthBarParent.transform, false);
        }
    }
    public void UpdateHealthBar(int current, int maxHealth)
    {
        current = Mathf.Max(0, current);
        for (int i = maxHealth - 1; i > 0; i--)
        {
            if (i > current)
            {
                if (i == 0) _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Broken Left", typeof(Sprite)) as Sprite;
                else if (i == maxHealth - 1) _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Broken Right", typeof(Sprite)) as Sprite;
                else _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Broken Middle", typeof(Sprite)) as Sprite;
            }
            else
            {
                if (i == 0) _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Left", typeof(Sprite)) as Sprite;
                else if (i == maxHealth - 1) _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Right", typeof(Sprite)) as Sprite;
                else _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Middle", typeof(Sprite)) as Sprite;
            }
        }
    }
    public void SpawnGameOverScreen()
    {
        GameObject gameOver = Instantiate((GameObject)Resources.Load("GameOverScreen"));
        gameOver.transform.position = Vector3.zero;
        gameOver.transform.SetParent(transform, false);
        List<Button> buttons = gameOver.GetComponentsInChildren<Button>().ToList();
        foreach (Button b in buttons)
        {
            b.onClick = new Button.ButtonClickedEvent();
            b.onClick.AddListener(() => LevelManager.instance.GoToMainMenu());
        }
    }
    public IEnumerator FlashWeaponOutOfRangeWarning()
    {
        List<Image> images = WeaponOutOfRangeParent.GetComponentsInChildren<Image>().ToList();
        int time = 50;
        WeaponOutOfRangeParent.SetActive(true);
        for (int i = 0; i < time / 2; i++)
        {
            foreach (Image img in images)
            {
                Color c = img.color;
                img.color = new Color(c.r, c.g, c.b, i / (time / 2f));
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
        for (int i = 0; i < time / 2; i++)
        {
            foreach (Image img in images)
            {
                Color c = img.color;
                img.color = new Color(c.r, c.g, c.b, 1 - i / (time / 2f));
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
        WeaponOutOfRangeParent.SetActive(false);
    }
    private void SaveWizards(bool b)
    {
        if(b)
        {
            _saveWizardsImage.sprite = Resources.Load("Art\\UI\\Save_Wizards_Green", typeof(Sprite)) as Sprite;
            _returnWizardsImage.sprite = Resources.Load("Art\\UI\\Return_Wizards_Brown", typeof(Sprite)) as Sprite;
            _returnWizardsButton.interactable = true;
        }
        else
        {
            _saveWizardsImage.sprite = Resources.Load("Art\\UI\\Save_Wizards_Brown", typeof(Sprite)) as Sprite;
            _returnWizardsImage.sprite = Resources.Load("Art\\UI\\Return_Wizards_Red", typeof(Sprite)) as Sprite;
            _returnWizardsButton.interactable = false;
        }
        Ref.PCon.SaveAllWizardPositions();
    }

    //  Emergency Brake
    private void EmergencyBrake(Toggle t)
    {
        print(t.isOn);
        ActivateEmergencyBrake(t.isOn);
    }
    public void ActivateEmergencyBrake(bool b)
    {
        EmergencyBrakeToggle.isOn = b;
        Ref.PCon.TMov.emergencyBrakeOn = b;
        if(b)
        {
            Ref.PCon.TMov._matchSpeed = false;
            _desiredSpeedSlider.value = 0;
            Ref.PCon.TMov.TankEmergencyBrakeEffects();
        }
    }

    //  Engine Level
    private void InitEngineLevel()
    {
        engineLevelSegments = new List<Image>();
        for(int i = 0; i < 5; i++)
        {
            GameObject engineSegment = Instantiate((GameObject) Resources.Load("EngineLevelSegment"));
            engineSegment.transform.parent = _engineLevel;
            engineSegment.transform.localScale = Vector3.one;
            Image engineSegmentImg = engineSegment.GetComponent<Image>();
            engineLevelSegments.Add(engineSegmentImg);
        }
    }
    public void UpdateEngineLevel(int level, int maxLevel)
    {
        //  Just in case of syncing issues, make sure we have a bar to update
        if (engineLevelSegments.Count == 0) InitEngineLevel();

        //  Init Empty

        for (int i = 0; i < 5; i++)
        {
            Image engineSegmentImg = engineLevelSegments[i];
            if (i == 0) engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Left_Empty", typeof(Sprite)) as Sprite;
            else if (i == 4) engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Right_Empty", typeof(Sprite)) as Sprite;
            else engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Middle_Empty", typeof(Sprite)) as Sprite;
        }

        //   Update the full ones

        for (int i = 0; i < level; i++)
        {
            Image engineSegmentImg = engineLevelSegments[i];
            if (i == 0) engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Left_Full", typeof(Sprite)) as Sprite;
            else if (i == 4) engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Right_Full", typeof(Sprite)) as Sprite;
            else engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Middle_Full", typeof(Sprite)) as Sprite;
        }

        Ref.PCon.TMov.engineLevelMultiplier = 1 + (level / (float)maxLevel);
    }

    //  Steering Wheel
    private void ToggleSteeringWheel()
    {
        if (steeringWheelOpen) CloseSteeringWheel(); 
        else  OpenSteeringWheel(); 
    }
    private void CloseSteeringWheel()
    {
        steeringWheelOpen = false;
        _steeringWheelObject.anchoredPosition = new Vector3(0, -540, 0);
        _steeringWheelObject.transform.parent = steeringWheelParent;
        _steeringWheelPrompt.SetActive(true);
    }
    private void OpenSteeringWheel()
    {
        steeringWheelOpen = true;
        _steeringWheelObject.anchoredPosition = new Vector3(0, -390, 0);
        _steeringWheelObject.transform.parent = steeringWheelParent;
        _steeringWheelPrompt.SetActive(true);
    }
    private void SteeringWheelTrackMouse()
    {
        //TODO: maybe have some cord connection the steering wheel to the bottom like it was wrenched off :)

        _rotateBackButton.gameObject.SetActive(false);
        _steeringWheelPrompt.SetActive(false);
        if (Input.GetKey(KeyCode.Mouse0)) _steeringWheelObject.transform.parent = transform;
        else
        {
            _steeringWheelObject.transform.parent = Ref.mouse.mouseGameObject.transform;
            _steeringWheelObject.anchoredPosition = Vector3.zero;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0)) holdTime = 0;
    }
}
