using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    // Health

    [Space(10)]
    [Header("Health")]
    public GameObject _healthBarParent;
    public List<Image> _allHPSegments;

    //  XRay 

    [Space(10)]
    [Header("X-Ray")]
    public Image _xrayImage; [HideInInspector]
    public bool _xrayOn;

    //  Save and return Wizards to Positions

    [Space(10)]
    [Header("Wizards")]
    public GameObject _wizardsList;
    public Button _saveWizardsButton;
    public Image _saveWizardsImage;
    public Button _returnWizardsButton;
    public Image _returnWizardsImage;
    private bool wizardsSaved;

    [Space(10)]
    [Header("Weapons")]
    public GameObject _weaponsList;

    //  Track Tank

    [Space(10)]
    [Header("Center on Tank")]
    public Button _trackPlayerTankButton;
    public Image _trackPlayerImage;


    //  MISC STUFF

    [Space(10)]
    [Header("Misc Stuff")]
    public Slider ZoomSlider;
    public HorizontalLayoutGroup _systems;
    public Button _pauseButton;
    public Button _xrayButton;
    public GameObject _pauseImage;
    public GameObject WeaponOutOfRangeParent;

    //  Double Clicks

    [HideInInspector]
    public float timeBetweenMouseClicks;
    [HideInInspector]
    public GameObject LastWizardOrWeaponClicked;

    //  Other UI Scripts

    [Space(10)]
    [Header("UI Scripts")]
    public Minimap _minimapScript;
    public Settings _settingsScript;
    public SteeringWheel _steeringWheelScript;
    public EngineUI _engineUIScript;
    public UpgradeScreen _upgradeScreen;

    private void Awake()
    {
        wizardsSaved = false;
        Ref.UI = this;
    }
    private void Start()
    {
        SaveWizards(wizardsSaved);
        timeBetweenMouseClicks = 0;
        InitButtonsSlidersToggles();
        _xrayOn = true;
        _pauseImage.SetActive(false);
        Events.instance.CheckDoubleClick += CheckDoubleClick;
    }

    private void CheckDoubleClick(GameObject obj)
    {
        GameObject objToTrack = obj;
        if (LastWizardOrWeaponClicked)
            if (LastWizardOrWeaponClicked.Equals(obj))
                if (timeBetweenMouseClicks < 0.25f)
                {
                    if (obj.TryGetComponent(out AUnit unit))
                    {
                        if (unit.PlayerUIWizardIcon) objToTrack = Ref.PCon.transform.root.gameObject;
                    }
                    if(obj.TryGetComponent(out AWeapon weapon))
                    {
                        if (!weapon.ShouldHitPlayer) objToTrack = Ref.PCon.transform.root.gameObject;
                    }
                    Ref.Cam.SetTrackedVehicleToObject(objToTrack.transform);
                    Ref.Cam.SetDesiredZoom(Ref.Cam.maxZoom);
                }
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
        
    }
    
    private void InitButtonsSlidersToggles()
    {
        //Buttons
        _pauseButton.onClick.AddListener(() => Ref.TM.TogglePauseWhilstPlaying());
        _xrayButton.onClick.AddListener(() => ToggleVision());
        _trackPlayerTankButton.onClick.AddListener(() => Ref.Cam.SetTrackedVehicleToPlayer());
        _returnWizardsButton.onClick.AddListener(() => Ref.PCon.ReturnAllWizardsToSavedPositions());
        _saveWizardsButton.onClick.AddListener(() => SaveWizards(true));

        //  Toggles

        //  Sliders
        ZoomSlider.onValueChanged.AddListener(delegate { Ref.Cam.SetZoom(ZoomSlider.value);});
    }
    
    public PlayerWeaponUI CreateWeaponUI(AWeapon iwp)
    {
        GameObject go = Instantiate((GameObject)Resources.Load("Weapons/PlayerWeaponUI"));
        PlayerWeaponUI wp = go.GetComponent<PlayerWeaponUI>();
        wp._weaponImage.sprite = iwp._weaponStats._UISprite;
        wp._UIWeaponName.text = iwp.SystemName;
        wp._index = iwp.WeaponUI.WeaponIndex;
        wp._weapon = iwp;
        wp._UIWeaponIndex.text = iwp.WeaponUI.WeaponIndex.ToString();

        go.transform.SetParent(_weaponsList.transform, false);
        return wp;
    }
    public PlayerWizardUI CreateWizardUI(AUnit unit)
    {
        GameObject go = Instantiate((GameObject)Resources.Load("Wizards/PlayerWizardUI"));
        PlayerWizardUI u = go.GetComponent<PlayerWizardUI>();
        u._wizardImage.sprite = unit.PlayerUIWizardIcon;
        u._UIWizardName.text = unit.UnitName;
        u._index = unit.Index;
        u._UIWizardKeybind.text = "CTRL + " + u._index.ToString();
        u.button.onClick = new Button.ButtonClickedEvent();
        u.button.onClick.AddListener(() => u.SelectWizard());
        u.wizard = unit;
        go.transform.SetParent(_wizardsList.transform, false);
        go.transform.localScale = new Vector3(-1, 1, 1);
        return u;
    }


    //  XRay

    private void ToggleVision()
    {
        TurnOnXRay(!_xrayOn);
        _xrayOn = !_xrayOn;
    }
    public void TurnOnXRay(bool xrayOn)
    {
        //  SHOW XRAY

        if (xrayOn)
        {
            _xrayImage.sprite = Resources.Load("Art/UI/XRayOn", typeof(Sprite)) as Sprite;

            //  Player

            if (Ref.PCon)
            {
                foreach (AWeapon wep in Ref.PCon.TWep.AWeaponArray)
                {
                    wep.SetOpacity(true);
                    wep.WeaponUI._weaponIndexText.transform.parent.gameObject.SetActive(true);
                }
                foreach(ASystem sys in Ref.PCon.TWep.ASystemArray)
                {
                    sys.SetOpacity(true);
                }
                Ref.PCon.TGeo.ShowRoof(false);
            }

            //  Enemies

            if (Ref.EM)
            {
                if (Ref.EM._enemyTanks.Count > 0)
                {
                    foreach (EnemyTankController g in Ref.EM._enemyTanks)
                    {
                        foreach (AWeapon wep in g.TWep.AWeaponArray)
                        {
                            wep.SetOpacity(true);
                        }
                        foreach (ASystem sys in g.TWep.ASystemArray)
                        {
                            sys.SetOpacity(true);
                        }
                        g.GetComponent<EnemyTankController>().TGeo.ShowRoof(false);
                    }
                }
            }

            //  Toggle Icons to be underneath roof

        }

        //  SHOW ROOF

        else
        {
            _xrayImage.sprite = Resources.Load("Art/UI/XRayOff", typeof(Sprite)) as Sprite;

            //  Player

            if (Ref.PCon)
            {
                foreach (AWeapon wep in Ref.PCon.TWep.AWeaponArray)
                {
                    wep.SetOpacity(false);
                    wep.WeaponUI._weaponIndexText.transform.parent.gameObject.SetActive(false);
                }
                foreach (ASystem sys in Ref.PCon.TWep.ASystemArray)
                {
                    sys.SetOpacity(false);
                }
                Ref.PCon.TGeo.ShowRoof(true);
            }

            //  Enemies

            if (Ref.EM)
            {
                if (Ref.EM._enemyTanks.Count > 0)
                {
                    foreach (EnemyTankController g in Ref.EM._enemyTanks)
                    {
                        foreach (AWeapon wep in g.TWep.AWeaponArray)
                        {
                            wep.SetOpacity(false);
                        }
                        foreach (ASystem sys in g.TWep.ASystemArray)
                        {
                            sys.SetOpacity(false);
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

            if (i == 0) img.sprite = Resources.Load("Art/UI/HP Bar/HP Segment Left", typeof(Sprite)) as Sprite;
            else if (i == maxHealth - 1) img.sprite = Resources.Load("Art/UI/HP Bar/HP Segment Right", typeof(Sprite)) as Sprite;
            else img.sprite = Resources.Load("Art/UI/HP Bar/HP Segment Middle", typeof(Sprite)) as Sprite;

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
                if (i == 0) _allHPSegments[i].sprite = Resources.Load("Art/UI/HP Bar/HP Segment Broken Left", typeof(Sprite)) as Sprite;
                else if (i == maxHealth - 1) _allHPSegments[i].sprite = Resources.Load("Art/UI/HP Bar/HP Segment Broken Right", typeof(Sprite)) as Sprite;
                else _allHPSegments[i].sprite = Resources.Load("Art/UI/HP Bar/HP Segment Broken Middle", typeof(Sprite)) as Sprite;
            }
            else
            {
                if (i == 0) _allHPSegments[i].sprite = Resources.Load("Art/UI/HP Bar/HP Segment Left", typeof(Sprite)) as Sprite;
                else if (i == maxHealth - 1) _allHPSegments[i].sprite = Resources.Load("Art/UI/HP Bar/HP Segment Right", typeof(Sprite)) as Sprite;
                else _allHPSegments[i].sprite = Resources.Load("Art/UI/HP Bar/HP Segment Middle", typeof(Sprite)) as Sprite;
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
            _saveWizardsImage.sprite = Resources.Load("Art/UI/Save_Wizards_Green", typeof(Sprite)) as Sprite;
            _returnWizardsImage.sprite = Resources.Load("Art/UI/Return_Wizards_Brown", typeof(Sprite)) as Sprite;
            _returnWizardsButton.interactable = true;
        }
        else
        {
            _saveWizardsImage.sprite = Resources.Load("Art/UI/Save_Wizards_Brown", typeof(Sprite)) as Sprite;
            _returnWizardsImage.sprite = Resources.Load("Art/UI/Return_Wizards_Red", typeof(Sprite)) as Sprite;
            _returnWizardsButton.interactable = false;
        }
        Ref.PCon.SaveAllWizardPositions();
    }

    //  Emergency Brake
   
    public void TrackingTank(bool b)
    {
        if (b) _trackPlayerImage.sprite = Resources.Load("Art/UI/TrackTankTrue", typeof (Sprite)) as Sprite;
        else _trackPlayerImage.sprite = Resources.Load("Art/UI/TrackTankFalse", typeof(Sprite)) as Sprite;
    }
}