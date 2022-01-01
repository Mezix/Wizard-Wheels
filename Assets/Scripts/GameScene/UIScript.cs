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
    public List<Image> _allHPSegments;

    //  XRay 
    public bool _xrayOn;
    public Image _xrayImage;

    //  MatchSpeed
    //public Image matchSpeedButtonBG;
    //public Button _matchSpeedButton;


    //  Double Clicks

    public float timeBetweenMouseClicks;
    public GameObject LastWizardOrWeaponClicked;


    //  Weapon Out Of Range

    public GameObject WeaponOutOfRangeParent;

    private void Awake()
    {
        _settingsOn = false;
        Ref.UI = this;
    }
    private void Start()
    {
        timeBetweenMouseClicks = 0;
        InitButtons();
        _xrayOn = true;
        _pauseImage.SetActive(false);
        CloseSettings();
        Events.instance.WizardOrWeaponClicked += CheckDoubleClick;
    }

    private void CheckDoubleClick(GameObject obj)
    {
        if(LastWizardOrWeaponClicked)
        if(LastWizardOrWeaponClicked.Equals(obj))
        if(timeBetweenMouseClicks < 0.25f)
        Ref.Cam.SetTrackedVehicleToObject(obj.transform);
        Ref.Cam.SetDesiredZoom(Ref.Cam.maxZoom);

        LastWizardOrWeaponClicked = obj;
        timeBetweenMouseClicks = 0;
    }

    private void Update()
    {
        timeBetweenMouseClicks += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.V))
        {
            ToggleVision();
        }
    }
    private void InitButtons()
    {
        _cruiseButton.onClick = new Button.ButtonClickedEvent();
        _cruiseButton.onClick.AddListener(() => Ref.PCon.TMov.ToggleCruise());
        //_matchSpeedButton.onClick = new Button.ButtonClickedEvent();
        //_matchSpeedButton.onClick.AddListener(() => AttemptToMatchSpeed());
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
        if(!TimeManager.paused) Ref.TM.UnfreezeTime();
        _settings.SetActive(false);
        _settingsOn = false;
    }
    public PlayerWeaponUI CreateWeaponUI(AWeapon iwp)
    {
        GameObject go = Instantiate((GameObject) Resources.Load("Weapons\\PlayerWeaponUI"));
        PlayerWeaponUI wp = go.GetComponent<PlayerWeaponUI>();
        wp._weaponImage.sprite = iwp._weaponStats._UISprite;
        wp._UIWeaponName.text = iwp.SystemName;
        wp._index = iwp.EnemyWepUI.WeaponIndex;
        wp._weapon = iwp;
        wp._UIWeaponIndex.text = iwp.EnemyWepUI.WeaponIndex.ToString();

        go.transform.SetParent(_weaponsList.transform,false);
        return wp;
    }
    public PlayerWizardUI CreateWizardUI(AUnit unit)
    {
        GameObject go = Instantiate((GameObject)Resources.Load("PlayerWizardUI"));
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
    }
    public void TurnOnCruiseMode(bool b)
    {
        if (b) _cruiseButton.image.color = Color.black;
        else _cruiseButton.image.color = Color.white;
    }
    
    /*public void SetMatchSpeedButton(int i)
    {
        if (i == 0)
        {
            matchSpeedButtonBG.color = Color.white;
        }
        else if (i == 1)
        {
            matchSpeedButtonBG.color = Color.red;
        }
        else
        {
            matchSpeedButtonBG.color = Color.black;

        }
    }*/

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
        if(_allHPSegments.Count > 0)
        {
            foreach(Image g in _allHPSegments) Destroy(g);
            _allHPSegments.Clear();
        }
        for(int i = 0; i < maxHealth; i++)
        {
            GameObject tmp = Instantiate((GameObject) Resources.Load("HPSegment"));
            Image img = tmp.GetComponent<Image>();

            if (i == 0) img.sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Left", typeof(Sprite)) as Sprite;
            else if(i == maxHealth-1) img.sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Right", typeof(Sprite)) as Sprite;
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
            if(i > current)
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

    public IEnumerator FlashWeaponOutOfRangeWarning()
    {
        List<Image> images = WeaponOutOfRangeParent.GetComponentsInChildren<Image>().ToList();
        int time = 50;
        WeaponOutOfRangeParent.SetActive(true);
        for(int i = 0; i < time/2; i++)
        {
            foreach(Image img in images)
            {
                Color c = img.color;
                img.color = new Color(c.r, c.g, c.b, i/(time/2f));
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
        for (int i = 0; i < time / 2; i++)
        {
            foreach (Image img in images)
            {
                Color c = img.color;
                img.color = new Color(c.r, c.g, c.b, 1 - i/(time/2f));
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
        WeaponOutOfRangeParent.SetActive(false);
    }
}
