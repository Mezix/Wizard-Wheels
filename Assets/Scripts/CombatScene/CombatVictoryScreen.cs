using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatVictoryScreen : MonoBehaviour
{
    public Button _continueButton;
    private void Awake()
    {
        //_continueButton.onClick.AddListener(() => LevelManager.instance.GoToMainMenu());
        _continueButton.onClick.AddListener(() => DataStorage.Singleton.FinishEvent());
    }
}
