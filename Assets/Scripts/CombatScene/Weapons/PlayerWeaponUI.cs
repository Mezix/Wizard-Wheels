using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerWeaponUI : MonoBehaviour
{
    [Header("Base Weapon UI")]
    public AWeapon _assignedWeapon;
    public Button _weaponSelectionButton;
    public Image _weaponImage;
    public GameObject _weaponNotInteractedWith;
    public GameObject _weaponNeedsToBeReparied;

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
    public Text _weaponAttacksPerSecond;
    public Text _weaponRange;
    public Text _weaponHull;

    [Header("Weapon Charge")]
    public Button _weaponChargeButtonForHovering;
    public GameObject _weaponChargeParentObj;
    public Image _weaponCharge;

    [Header("Manual Fire")]
    public bool _hasManualFire;
    public Button _manualFireButton;

    [Header("Weapon Selected")]
    public GameObject _weaponSelected;

    //Hovering
    public Transform _tempDraggedObject;
    private bool mouseHoveringOverSelectionButton;
    private bool mouseHoveringOverStats;
    private bool mouseHoveringOverCharge;

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
            ShowExtended(false);
            MoveWepUIToNewIndex();
        }
        else
        {
            if (mouseHoveringOverStats || mouseHoveringOverCharge || mouseHoveringOverSelectionButton) ShowExtended(true);
            else ShowExtended(false);
        }
    }


    private void ShowExtended(bool showExtended)
    {
        _extendableStatsParentObj.SetActive(showExtended);
        _weaponChargeParentObj.SetActive(showExtended);
    }

    public void Init(AWeapon wep)
    {
        _assignedWeapon = wep;
        _assignedWeapon.PlayerUIWep = this;
        _weaponImage.sprite = _assignedWeapon._weaponStats._UISprite;

        _weaponName.text = _assignedWeapon._weaponStats._weaponName;
        _WeaponDamage.text = _assignedWeapon._weaponStats._damage.ToString();
        _weaponRange.text = _assignedWeapon._weaponStats._lockOnRange.ToString();
        _weaponAttacksPerSecond.text = _assignedWeapon._weaponStats._attacksPerSecond.ToString();
        UpdateHP();

        _hasManualFire = _assignedWeapon._isManualFire;
        _manualFireButton.gameObject.SetActive(_hasManualFire);

        _index = _assignedWeapon.WeaponUI.WeaponIndex;
        _UIWeaponIndex.text = _index.ToString();

        WeaponUISelected(false);
        ShowExtended(false);
    }

    public void SelectWeapon()
    {
        if (!Input.GetKey(KeyCode.LeftShift)) REF.PCon.TWep.DeselectAllWeapons();
        Events.instance.DoubleClickAttempted(_assignedWeapon.gameObject);
        if (_assignedWeapon != null)
        {
            if (_assignedWeapon.WeaponEnabled)
            {
                WeaponUISelected(true);
                _assignedWeapon.WeaponSelected = true;
            }
        }
    }
    public void WeaponIsBeingInteractedWith(bool interactionStatus)
    {
        _weaponSelectionButton.interactable = !interactionStatus;
        _weaponNotInteractedWith.SetActive(!interactionStatus);
    }
    public void WeaponUISelected(bool isSelected)
    {
        _weaponSelected.SetActive(isSelected);
    }
    private void UpdateHP()
    {
        _weaponHull.text = _assignedWeapon.RoomPosForInteraction.ParentRoom._currentHP + "/" + _assignedWeapon.RoomPosForInteraction.ParentRoom._maxHP;
    }
    private void InitUI()
    {
        _weaponSelectionButton.onClick.AddListener(() => SelectWeapon());

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
}
