using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIScript : MonoBehaviour
{
    //  Canvas
    [HideInInspector]
    public Canvas _canvas;

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
    public List<PlayerWeaponUI> AllUIWeapons = new List<PlayerWeaponUI>();
    public HorizontalLayoutGroup _weaponsHLayoutGroup;
    public Image _WeaponHolderBG;

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
    public Button _finishEventButton;

    //  Double Clicks

    [HideInInspector]
    public float timeBetweenMouseClicks;
    [HideInInspector]
    public GameObject LastWizardOrWeaponClicked;

    //  Other UI Scripts

    [Space(10)]
    [Header("UI Scripts")]
    public Minimap _minimapScript;
    public SettingsScript _settingsScript;
    public SteeringWheel _steeringWheelScript;
    public EngineUI _engineUIScript;
    public UpgradeScreen _upgradeScreen;
    public InventoryUI _inventoryUI;
    public BossUI _bossUI;

    private void Awake()
    {
        wizardsSaved = false;
        _canvas = GetComponent<Canvas>();
        REF.CombatUI = this;
    }
    private void Start()
    {
        _settingsScript.CloseSettings();
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
                        if (unit.PlayerUIWizardIcon) objToTrack = REF.PCon.transform.root.gameObject;
                    }
                    if(obj.TryGetComponent(out AWeapon weapon))
                    {
                        if (!weapon.ShouldHitPlayer) objToTrack = REF.PCon.transform.root.gameObject;
                    }
                    REF.Cam.SetTrackedVehicleToObject(objToTrack.transform);
                    REF.Cam.SetDesiredZoom(REF.Cam.maxZoom);
                }
        LastWizardOrWeaponClicked = obj;
        timeBetweenMouseClicks = 0;
    }


    private void Update()
    {
        timeBetweenMouseClicks += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleVision();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            REF.Cam.SetTrackedVehicleToPlayer();
        }
    }
    
    private void InitButtonsSlidersToggles()
    {
        //Buttons
        _pauseButton.onClick.AddListener(() => REF.TM.TogglePauseWhilstPlaying());
        _xrayButton.onClick.AddListener(() => ToggleVision());
        _trackPlayerTankButton.onClick.AddListener(() => REF.Cam.SetTrackedVehicleToPlayer());
        _returnWizardsButton.onClick.AddListener(() => REF.PCon.ReturnAllWizardsToSavedPositions());
        _saveWizardsButton.onClick.AddListener(() => SaveWizards(true));
        _finishEventButton.onClick.AddListener(() => DataStorage.Singleton.FinishEvent());

        //  Toggles

        //  Sliders
        ZoomSlider.onValueChanged.AddListener(delegate { REF.Cam.SetZoom(ZoomSlider.value);});
    }
    
    public void CreateWeaponUI(AWeapon weapon)
    {
        weapon._weaponHoveringUI = Instantiate(Resources.Load(GS.WeaponPrefabs("WeaponHoveringUI"), typeof(WeaponHoveringUI)) as WeaponHoveringUI);
        weapon._weaponHoveringUI.transform.localPosition = weapon.RotatablePart.transform.localPosition;
        weapon._weaponHoveringUI.transform.SetParent(weapon.transform, false);
        HM.RotateLocalTransformToAngle(weapon._weaponHoveringUI.transform, new Vector3(0, 0, -90));

        PlayerWeaponUI playerWeaponUI = Instantiate(Resources.Load(GS.WeaponPrefabs("PlayerWeaponUI"),  typeof (PlayerWeaponUI)) as PlayerWeaponUI, _weaponsHLayoutGroup.transform, false);
        playerWeaponUI.Init(weapon);
        float holderSize = (_weaponsHLayoutGroup.transform.childCount * 60) +                // cumulative size of all weapons
            (_weaponsHLayoutGroup.transform.childCount - 1) * _weaponsHLayoutGroup.spacing + // cumulative spacing between each wep ui
                                                                                         10; // spacing to left and right to encompass all weapons
        _WeaponHolderBG.rectTransform.sizeDelta = new Vector2(holderSize, 70);
        AllUIWeapons.Add(playerWeaponUI);
    }
    public void ReorderWeapons()
    {
        AllUIWeapons = new List<PlayerWeaponUI>();
        REF.PCon.TWep.AWeaponArray = new List<AWeapon>();
        for(int i = 0; i < _weaponsHLayoutGroup.transform.childCount; i++)
        {
            PlayerWeaponUI wep = _weaponsHLayoutGroup.transform.GetChild(i).GetComponent<PlayerWeaponUI>();
            wep._index = i + 1;
            wep._UIWeaponIndex.text = wep._index.ToString();

            wep._assignedWeapon.WeaponUI.WeaponIndex = wep._index;
            wep._assignedWeapon.WeaponUI._weaponIndexText.text = wep._index.ToString();

            AllUIWeapons.Add(wep);
            REF.PCon.TWep.AWeaponArray.Add(wep._assignedWeapon);
        }
    }

    public PlayerWizardUI CreateWizardUI(AUnit unit)
    {
        PlayerWizardUI wizUI = Instantiate(Resources.Load(GS.UIPrefabs("PlayerWizardUI"), typeof(PlayerWizardUI)) as PlayerWizardUI);
        wizUI._wizardImage.sprite = unit.PlayerUIWizardIcon;
        wizUI._UIWizardName.text = unit.UnitName;
        wizUI._index = unit.Index;
        wizUI._UIWizardKeybind.text = "CTRL + " + wizUI._index.ToString();
        wizUI.button.onClick = new Button.ButtonClickedEvent();
        wizUI.button.onClick.AddListener(() => wizUI.SelectWizard());
        wizUI.wizard = unit;
        wizUI.transform.SetParent(_wizardsList.transform, false);
        return wizUI;
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
            _xrayImage.sprite = Resources.Load(GS.UIGraphics("XRayOn"), typeof(Sprite)) as Sprite;

            //  Player

            if (REF.PCon)
            {
                foreach (AWeapon wep in REF.PCon.TWep.AWeaponArray)
                {
                    wep.SetOpacity(true);
                    wep.WeaponUI._weaponIndexText.transform.parent.gameObject.SetActive(true);
                }
                foreach(ASystem sys in REF.PCon.TWep.ASystemArray)
                {
                    sys.SetOpacity(true);
                }
                REF.PCon.TGeo.ShowRoof(false);
            }

            //  Enemies

            if (REF.EM)
            {
                if (REF.EM._enemyTanks.Count > 0)
                {
                    foreach (EnemyTankController g in REF.EM._enemyTanks)
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
            _xrayImage.sprite = Resources.Load(GS.UIGraphics("XRayOff"), typeof(Sprite)) as Sprite;

            //  Player

            if (REF.PCon)
            {
                foreach (AWeapon wep in REF.PCon.TWep.AWeaponArray)
                {
                    wep.SetOpacity(false);
                    wep.WeaponUI._weaponIndexText.transform.parent.gameObject.SetActive(false);
                }
                foreach (ASystem sys in REF.PCon.TWep.ASystemArray)
                {
                    sys.SetOpacity(false);
                }
                REF.PCon.TGeo.ShowRoof(true);
            }

            //  Enemies

            if (REF.EM)
            {
                if (REF.EM._enemyTanks.Count > 0)
                {
                    foreach (EnemyTankController g in REF.EM._enemyTanks)
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
            Image img = Instantiate(Resources.Load(GS.UIPrefabs("HPSegment"),  typeof (Image)) as Image);

            if (i == 0) img.sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Left"), typeof(Sprite)) as Sprite;
            else if (i == maxHealth - 1) img.sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Right"), typeof(Sprite)) as Sprite;
            else img.sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Middle"), typeof(Sprite)) as Sprite;

            _allHPSegments.Add(img);
            img.transform.SetParent(_healthBarParent.transform, false);
        }
    }
    public void UpdateHealthBar(int current, int maxHealth)
    {
        current = Mathf.Max(0, current);
        for (int i = maxHealth - 1; i > 0; i--)
        {
            if (i > current)
            {
                if (i == 0) _allHPSegments[i].sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Broken Left"), typeof(Sprite)) as Sprite;
                else if (i == maxHealth - 1) _allHPSegments[i].sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Broken Right"), typeof(Sprite)) as Sprite;
                else _allHPSegments[i].sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Broken Middle"), typeof(Sprite)) as Sprite;
            }
            else
            {
                if (i == 0) _allHPSegments[i].sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Left"), typeof(Sprite)) as Sprite;
                else if (i == maxHealth - 1) _allHPSegments[i].sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Right"), typeof(Sprite)) as Sprite;
                else _allHPSegments[i].sprite = Resources.Load(GS.UIGraphics("HP Bar/HP Segment Middle"), typeof(Sprite)) as Sprite;
            }
        }
    }
    public void SpawnGameOverScreen()
    {
        Instantiate(Resources.Load(GS.UIPrefabs("CombatDefeatScreen"), typeof (GameObject)) as GameObject, transform, false);
        REF.TM.TriggerGradualSlowdown(0.2f);
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
            _saveWizardsImage.sprite = Resources.Load(GS.UIGraphics("Save_Wizards_Green"), typeof(Sprite)) as Sprite;
            _returnWizardsImage.sprite = Resources.Load(GS.UIGraphics("Return_Wizards_Brown"), typeof(Sprite)) as Sprite;
            _returnWizardsButton.interactable = true;
        }
        else
        {
            _saveWizardsImage.sprite = Resources.Load(GS.UIGraphics("Save_Wizards_Brown"), typeof(Sprite)) as Sprite;
            _returnWizardsImage.sprite = Resources.Load(GS.UIGraphics("Return_Wizards_Red"), typeof(Sprite)) as Sprite;
            _returnWizardsButton.interactable = false;
        }
        REF.PCon.SaveAllWizardPositions();
    }
    public void TrackingTank(bool b)
    {
        if (b) _trackPlayerImage.sprite = Resources.Load(GS.UIGraphics("TrackTankTrue"), typeof (Sprite)) as Sprite;
        else _trackPlayerImage.sprite = Resources.Load(GS.UIGraphics("TrackTankFalse"), typeof(Sprite)) as Sprite;
    }


    // BOSS
    public void SpawnBossHealth(EnemyTankController enemyTank)
    {
        _bossUI.Init(enemyTank);
    }
}