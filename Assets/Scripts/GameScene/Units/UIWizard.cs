﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWizard : MonoBehaviour
{
    public IUnit wizard;
    public Image _wizardImage;
    public int _index;
    public Text _UIWizardName;
    public Text _UIWizardKeybind;
    public Image _UIWizardHealthbarFill;
    public Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    public void SelectWizard()
    {
        //first deselect all other wizards unless shift is pressed
        if (!Input.GetKey(KeyCode.LeftShift)) Ref.PCon.DeselectAllWizards();

        if (wizard != null) wizard.UnitSelected = true;
    }
    public void UpdateButton(bool selected)
    {
        if (selected) _wizardImage.color = Color.black;
        else _wizardImage.color = Color.white;
    }
}
