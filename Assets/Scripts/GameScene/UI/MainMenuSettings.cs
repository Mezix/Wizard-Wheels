using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSettings : MonoBehaviour
{
    //  Settings

    public GameObject _settingsObj;
    public Button _closeSettingsButton;
    [HideInInspector]
    public bool _settingsOn;
    public Button _openSettingsButton;
    public Button _applySettingsButton;
    public Button _revertSettingsButton;
    public Toggle _fullScreenToggle;

    //  FULLSCREEN AND RESOLUTION

    [Space(30)]
    public bool _fullscreen;
    private Resolution[] resolutions;
    public Text _resolutionText;
    [HideInInspector]
    public int _tempSelectedResolution;
    [HideInInspector]
    public int _currentlySelectedResolution;
    public Button _nextResolutionButton;
    public Button _previousResolutionButton;

    private void Awake()
    {
        _openSettingsButton.onClick.AddListener(() => ToggleSettings());
        _closeSettingsButton.onClick.AddListener(() => CloseSettings());
        _nextResolutionButton.onClick.AddListener(() => NextResolution());
        _previousResolutionButton.onClick.AddListener(() => PreviousResolution());
        _applySettingsButton.onClick.AddListener(() => ApplySettings());
        _revertSettingsButton.onClick.AddListener(() => RevertSettings());
        _fullScreenToggle.onValueChanged.AddListener(delegate { SetFullscreen(_fullScreenToggle); });
    }
    private void Start()
    {
        _settingsOn = false;
        CloseSettings();
        InitResolutions();
    }

    //  SETTINGS

    public void ToggleSettings()
    {
        if (!_settingsOn) OpenSettings();
        else CloseSettings();
    }
    public void OpenSettings()
    {
        REF.TM.FreezeTime();
        _settingsObj.SetActive(true);
        _settingsOn = true;
    }
    public void CloseSettings()
    {
        if (!TimeManager.paused) REF.TM.UnfreezeTime();
        _settingsObj.SetActive(false);
        _settingsOn = false;
    }

    //RESOLUTION AND FULLSCREEN

    private void InitResolutions()
    {
        resolutions = new Resolution[8];

        resolutions[0].width = 600;
        resolutions[0].height = 480;

        resolutions[1].width = 1024;
        resolutions[1].height = 768;

        resolutions[2].width = 1152;
        resolutions[2].height = 768;

        resolutions[3].width = 1280;
        resolutions[3].height = 960;

        resolutions[4].width = 1366;
        resolutions[4].height = 768;

        resolutions[5].width = 1600;
        resolutions[5].height = 900;

        resolutions[6].width = 1920;
        resolutions[6].height = 1080;

        resolutions[7].width = 3840;
        resolutions[7].height = 2160;

        _currentlySelectedResolution = 5;
        _tempSelectedResolution = 5;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                _currentlySelectedResolution = i;
                SetResolutionText(_currentlySelectedResolution);
            }
        }
    }

    public void NextResolution()
    {
        if (_tempSelectedResolution < resolutions.Length - 1)
        {
            _tempSelectedResolution++;
        }
        else
        {
            _tempSelectedResolution = 0;
        }
        SetResolutionText(_tempSelectedResolution);
    }

    public void PreviousResolution()
    {
        if (_tempSelectedResolution > 0)
        {
            _tempSelectedResolution--;
        }
        else
        {
            _tempSelectedResolution = resolutions.Length - 1;
        }
        SetResolutionText(_tempSelectedResolution);
    }
    public void SetResolutionText(int index)
    {
        _resolutionText.text = resolutions[index].width.ToString() + "x" + resolutions[index].height.ToString();
    }
    public void SetFullscreen(bool isFullscreen)
    {
        _fullscreen = isFullscreen;
    }
    public void ApplySettings()
    {
        Screen.fullScreen = _fullscreen;
        _currentlySelectedResolution = _tempSelectedResolution;
        Screen.SetResolution(resolutions[_currentlySelectedResolution].width, resolutions[_currentlySelectedResolution].height, _fullscreen);
    }
    public void RevertSettings()
    {

    }
}
