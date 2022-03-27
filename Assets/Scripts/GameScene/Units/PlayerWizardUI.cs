using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWizardUI : MonoBehaviour
{
    public AUnit wizard;
    public Image _wizardImage;
    public int _index;
    public Text _UIWizardName;
    public Text _UIWizardKeybind;
    public Image _UIWizardHealthbarFill;
    public Button button;
    public GameObject _wizardSelected;
    private void Awake()
    {
        button = GetComponent<Button>();
        UpdateButton(false);
    }
    public void SelectWizard()
    {
        if (Ref.PCon._dying || Ref.PCon._dead) return;

        Events.instance.WizOrWepClicked(wizard.gameObject);
        //first deselect all other wizards unless shift is pressed
        if (!Input.GetKey(KeyCode.LeftShift)) Ref.PCon.DeselectAllWizards();

        if (wizard != null)
        {
            wizard.UnitSelected = true;
            UpdateButton(true);
        }
    }
    public void UpdateButton(bool selected)
    {
        _wizardSelected.SetActive(selected);
    }
}
