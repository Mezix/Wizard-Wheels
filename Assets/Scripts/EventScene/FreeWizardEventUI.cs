using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class FreeWizardEventUI : MonoBehaviour
{
    public GameObject _allObjects;
    public Button _finishEventButton;
    public Text _choosePromptText;
    public HorizontalLayoutGroup _wizardSelecter;
    private FreeWizardEventWizardSlot _wizSlotPrefab;
    private List<FreeWizardEventWizardSlot> _spawnedWizardSlots = new List<FreeWizardEventWizardSlot>();
    private void Awake()
    {
        _finishEventButton.onClick.AddListener(() => FinishEvent());
        _finishEventButton.gameObject.SetActive(false);
        _choosePromptText.gameObject.SetActive(true);

        _wizSlotPrefab = Resources.Load(GS.UIPrefabs("FreeWizardEventWizardSlot"), typeof(FreeWizardEventWizardSlot)) as FreeWizardEventWizardSlot;
        List<AUnit> allTypesOfWizardToSpawn = Resources.LoadAll(GS.Wizards(), typeof(InventoryItem)).Cast<AUnit>().ToList();

        for (int i = 0; i < 3; i++)
        {
            FreeWizardEventWizardSlot slot = Instantiate(_wizSlotPrefab, _wizardSelecter.transform, false);
            int wizToSelect = UnityEngine.Random.Range(0, allTypesOfWizardToSpawn.Count);

            AUnit wizToSpawn = allTypesOfWizardToSpawn[wizToSelect];
            slot._wizardImage.sprite = wizToSpawn.PlayerUIWizardIcon;
            slot._wizardName.text = wizToSpawn._unitStats._unitClass.ToString();
            slot._selectWizardButton.onClick.AddListener(() => SelectWizard(slot));
            _spawnedWizardSlots.Add(slot);

            WizardData newWizardData = new WizardData
            {
                WizType = wizToSpawn.UnitClass,
                RoomPositionX = -1,
                RoomPositionY = -1
            };
            slot._wizData = newWizardData;

            allTypesOfWizardToSpawn.Remove(wizToSpawn);
        }
    }

    private void FinishEvent()
    {
        SavePlayerData.SavePlayer(DataStorage.Singleton.saveSlot, DataStorage.Singleton.playerData);
        DataStorage.Singleton.FinishEvent();
    }

    public void Show(bool show)
    {
        _allObjects.SetActive(show);
    }

    public void Init()
    {
    }

    public void SelectWizard(FreeWizardEventWizardSlot slot)
    {
        _finishEventButton.gameObject.SetActive(true);
        foreach(FreeWizardEventWizardSlot wizSlot in _spawnedWizardSlots)
        {
            wizSlot.gameObject.SetActive(false);
        }
        slot.gameObject.SetActive(true);
        slot._selectWizardButton.enabled = false;
        _choosePromptText.gameObject.SetActive(false);

        DataStorage.Singleton.AddWizard(slot._wizData);
    }
}
