using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatDefeatScreen : MonoBehaviour
{
    public Button _returnToMainMenuButton;
    private void Awake()
    {
        _returnToMainMenuButton.onClick.AddListener(() => LevelManager.instance.GoToMainMenu());
    }
}
