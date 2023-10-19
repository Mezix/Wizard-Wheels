using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AWeapon;

public class PlayerWeaponUI : MonoBehaviour
{
    [Header("Base Weapon UI")]
    public AWeapon _assignedWeapon;
    public Button _weaponSelectionButton;
    public Image _weaponImage;
    public GameObject _weaponNotInteractedWith;
    public GameObject _weaponNeedsToBeRepaired;

    [Header("Hotkey")]
    public GameObject _hotkeyParentObj;
    public bool draggingObject;
    public Button _hotkeyButton;
    public int _index;
    public Text _UIWeaponIndex;
    public GameObject _hotkeyFill;
    public GameObject _hotkeyShadow;

    [Header("Extendable Stats")]
    public GameObject _extendableStatsParentObj;
    public Button _statsButtonForHovering;
    public Text _weaponName;
    public Text _WeaponDamage;
    public Text _timeBetweenAttacks;
    public Text _weaponRange;
    public Text _weaponHull;

    [Header("Weapon Charge")]
    public Button _weaponChargeButtonForHovering;
    public GameObject _weaponChargeParentObj;
    public Image _weaponChargeImage;
    public Slider _weaponChargeSlider;
    public Gradient _weaponChargeGradient;
    public Color _chargeColor;

    [Header("Manual Fire")]
    public Button _manualFireButton;

    [Header("Weapon Selected")]
    public List<Image> _selectedSprites;
    public Color _selectedColor;

    //Hovering
    public Transform _tempDraggedObject;
    private bool mouseHoveringOverSelectionButton;
    private bool mouseHoveringOverStats;
    private bool mouseHoveringOverCharge;
    private bool mouseHoveringOverManualFire;

    private void Awake()
    {
        InitUI();
    }

    private void Update()
    {
        WeaponUIBehaviour();
    }
    private void WeaponUIBehaviour()
    {
        if (draggingObject)
        {
            ShowHoveredUI(false);
            MoveWepUIToNewIndex();
        }
        else
        {
            if (mouseHoveringOverStats || mouseHoveringOverCharge || mouseHoveringOverSelectionButton) ShowHoveredUI(true);
            else
            {
                if (!mouseHoveringOverManualFire) ShowHoveredUI(false);
            }
        }
    }


    private void ShowHoveredUI(bool showExtended)
    {
        _extendableStatsParentObj.SetActive(showExtended);
        _weaponChargeParentObj.SetActive(showExtended);

        _assignedWeapon._weaponHoveringUI.ShowUI(showExtended);
        if(showExtended) _assignedWeapon._weaponHoveringUI.SetRotation(HM.WrapAngle(_assignedWeapon.RotatablePart.localRotation.eulerAngles.z), HM.WrapAngle(_assignedWeapon.AngleToAimAt));
    }

    public void Init(AWeapon wep)
    {
        _assignedWeapon = wep;
        _assignedWeapon.PlayerWepUI = this;
        _weaponImage.sprite = _assignedWeapon._weaponStats._UISprite;

        _weaponName.text = _assignedWeapon._weaponStats._weaponName;

        if(_assignedWeapon._projectileSpots.Count > 1) _WeaponDamage.text = _assignedWeapon._projectileSpots.Count + "x" + _assignedWeapon._weaponStats._damage.ToString();
        else _WeaponDamage.text = _assignedWeapon._weaponStats._damage.ToString();

        if(_assignedWeapon._weaponStats._lockOnRange == -1) _weaponRange.text = "∞";
        else _weaponRange.text = _assignedWeapon._weaponStats._lockOnRange.ToString();
        _timeBetweenAttacks.text = _assignedWeapon.TimeBetweenAttacks.ToString();
        UpdateHP();

        _index = _assignedWeapon.WeaponUI.WeaponIndex;
        _UIWeaponIndex.text = _index.ToString();

        WeaponUISelected(false);
        ShowHoveredUI(false);
    }

    public void SelectWeapon()
    {
        if (!Input.GetKey(KeyCode.LeftShift)) REF.PCon.TWep.DeselectAllWeapons();
        Events.instance.DoubleClickAttempted(_assignedWeapon.gameObject);
        if (_assignedWeapon)
        {
            if (_assignedWeapon.WeaponEnabled)
            {
                WeaponUISelected(true);
                _assignedWeapon.WeaponSelected = true;
            }
        }
    }
    public void DeselectWeapon()
    {
        WeaponUISelected(false);
        if (_assignedWeapon) _assignedWeapon.WeaponSelected = false;
    }
    public void ManualFire()
    {
        _assignedWeapon.ManualFire();
    }
    public void WeaponIsBeingInteractedWith(bool interactionStatus)
    {
        _weaponSelectionButton.interactable = interactionStatus;
        _weaponNotInteractedWith.SetActive(!interactionStatus);
    }
    public void WeaponUISelected(bool isSelected)
    {
        foreach(Image obj in _selectedSprites)
        {
            obj.gameObject.SetActive(isSelected);
            obj.color = _selectedColor;
        }
    }
    public void UpdateHP()
    {
        _weaponHull.text = _assignedWeapon.RoomPosForInteraction.ParentRoom._currentHP + "/" + _assignedWeapon.RoomPosForInteraction.ParentRoom._maxHP;
        if (_assignedWeapon.RoomPosForInteraction.ParentRoom._currentHP < _assignedWeapon.RoomPosForInteraction.ParentRoom._maxHP) _weaponNeedsToBeRepaired.SetActive(true);
        else _weaponNeedsToBeRepaired.SetActive(false);
    }

    // Drag Weapon UI

    private void InitiateDrag()
    {
        _tempDraggedObject = Instantiate(transform, REF.mouse._cursorTransform, true);
        _tempDraggedObject.GetComponent<PlayerWeaponUI>().enabled = false;
        _tempDraggedObject.SetAsFirstSibling();

        MakeTransparent(true);
    }

    private void MakeTransparent(bool isTransparent)
    {
        Image[] allImages = GetComponentsInChildren<Image>();
        float opacity = 1;
        if (isTransparent) opacity = 0.25f;
        foreach(Image i in allImages)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, opacity);
        }
    }

    private void PlaceWeaponUIInSpot()
    {
        if (_tempDraggedObject) Destroy(_tempDraggedObject.gameObject);
        MakeTransparent(false);
    }
    private void MoveWepUIToNewIndex()
    {
        if(_tempDraggedObject)
        {
            Vector3 relativePos = REF.CombatUI._weaponsHLayoutGroup.transform.InverseTransformPoint(_tempDraggedObject.position);
            if(relativePos.x > 0 && relativePos.y < -15 && relativePos.y > -120f)
            {
                float idx = relativePos.x / 64f;
                int newIndex = 1 + Mathf.FloorToInt(idx);
                if (_index != newIndex)
                {
                    transform.SetSiblingIndex(newIndex-1);
                    REF.CombatUI.ReorderWeapons();
                }
            }
        }
    }


    private void InitUI()
    {
        _weaponSelectionButton.onClick.AddListener(() => SelectWeapon());
        _manualFireButton.onClick.AddListener(() => ManualFire());

        // Hotkey

        EventTrigger trigger = _hotkeyButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerDown;
        enter.callback.AddListener((data) => { draggingObject = true; InitiateDrag(); });
        trigger.triggers.Add(enter);

        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerUp;
        enter.callback.AddListener((data) => { draggingObject = false; PlaceWeaponUIInSpot(); });
        trigger.triggers.Add(enter);

        //  Stats

        trigger = _weaponSelectionButton.gameObject.AddComponent<EventTrigger>();
        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) => { mouseHoveringOverSelectionButton = true; });
        trigger.triggers.Add(enter);

        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerExit;
        enter.callback.AddListener((data) => { mouseHoveringOverSelectionButton = false; });
        trigger.triggers.Add(enter);


        //  Stats

        trigger = _statsButtonForHovering.gameObject.AddComponent<EventTrigger>();
        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) => { mouseHoveringOverStats = true; });
        trigger.triggers.Add(enter);

        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerExit;
        enter.callback.AddListener((data) => { mouseHoveringOverStats = false; });
        trigger.triggers.Add(enter);

        // charge

        trigger = _weaponChargeButtonForHovering.gameObject.AddComponent<EventTrigger>();
        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) => { mouseHoveringOverCharge = true; });
        trigger.triggers.Add(enter);

        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerExit;
        enter.callback.AddListener((data) => { mouseHoveringOverCharge = false; });
        trigger.triggers.Add(enter);

        //  ManualFire

        trigger = _manualFireButton.gameObject.AddComponent<EventTrigger>();
        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) => { mouseHoveringOverManualFire = true; });
        trigger.triggers.Add(enter);

        enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerExit;
        enter.callback.AddListener((data) => { mouseHoveringOverManualFire = false; });
        trigger.triggers.Add(enter);
    }

    public void SetCharge(float fillPct, FiringStatus chargeMode)
    {
        if(chargeMode.Equals(FiringStatus.Reloading))
        {
            _weaponChargeImage.color = _weaponChargeGradient.Evaluate(fillPct);
            _weaponChargeSlider.value = fillPct;
        }
        else if(chargeMode.Equals(FiringStatus.Charging))
        {
            _weaponChargeImage.color = _chargeColor;
            _weaponChargeSlider.value = 1 - fillPct;
        }

        if (_weaponChargeSlider.value >= 1) _manualFireButton.interactable = true;
        else _manualFireButton.interactable = false;
    }
}
