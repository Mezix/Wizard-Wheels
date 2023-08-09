using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeWizardEventUI : MonoBehaviour
{
    public GameObject _allObjects;
    public Button _finishEventButton;
    public Text _choosePromptText;
    public HorizontalLayoutGroup _wizardSelecter;
    private FreeWizardEventWizardSlot _wizSlotPrefab;
    private List<FreeWizardEventWizardSlot> _spawnedWizardSlots = new List<FreeWizardEventWizardSlot>();

    public List<GameObject> _allTypesOfWizardToSpawn;
    private void Awake()
    {
        _finishEventButton.onClick.AddListener(() => DataStorage.Singleton.FinishEvent());
        _finishEventButton.gameObject.SetActive(false);
        _choosePromptText.gameObject.SetActive(true);

        _wizSlotPrefab = Resources.Load(GS.UIPrefabs("FreeWizardEventWizardSlot"), typeof(FreeWizardEventWizardSlot)) as FreeWizardEventWizardSlot;
        for (int i = 0; i < 3; i++)
        {
            FreeWizardEventWizardSlot slot = Instantiate(_wizSlotPrefab, _wizardSelecter.transform, false);
            int wizToSelect = UnityEngine.Random.Range(0, _allTypesOfWizardToSpawn.Count);
            AUnit wizToSpawn = _allTypesOfWizardToSpawn[wizToSelect].GetComponent<AUnit>();
            slot._wizardImage.sprite = wizToSpawn.PlayerUIWizardIcon;
            slot._wizardName.text = wizToSpawn._unitStats._unitClass;
            slot._selectWizardButton.onClick.AddListener(() => SelectWizard(slot));
            _allTypesOfWizardToSpawn.Remove(wizToSpawn.gameObject);
            _spawnedWizardSlots.Add(slot);
        }
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
    }
}
