﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngineUI : MonoBehaviour
{
    //  Engine
    public HorizontalLayoutGroup _engineLevelLayoutGroup;
    public Animator _engineAnimator;
    private List<Image> engineLevelSegments;
    public Button _cruiseButton;
    public Slider _currentSpeedSlider;
    public Slider _desiredSpeedSlider;
    public Toggle _emergencyBrakeToggle;

    public AudioSource _engineIdleSound;
    public AudioSource _engineStartingSound;
    public AudioSource _engineStoppingSound;
    public bool engineSoundOn;

    //  MatchSpeed

    [Space(10)]
    [Header("Match Speed Button")]
    public Image _matchSpeedImage;
    public Button _unmatchSpeedButton;

    private void Awake()
    {
        _cruiseButton.onClick.AddListener(() => Ref.PCon.TMov.ToggleCruise());
        _emergencyBrakeToggle.onValueChanged.AddListener(delegate { EmergencyBrake(_emergencyBrakeToggle); });
        _unmatchSpeedButton.onClick.AddListener(() => UnmatchSpeedUI());
    }
    private void EmergencyBrake(Toggle t)
    {
        ActivateEmergencyBrake(t.isOn);
    }
    private void Start()
    {
        InitEngineLevel();
    }
    public void ActivateEmergencyBrake(bool b)
    {
        _emergencyBrakeToggle.isOn = b;
        Ref.PCon.TMov.emergencyBrakeOn = b;
        if (b)
        {
            Ref.PCon.TMov._matchSpeed = false;
            _desiredSpeedSlider.value = 0;
            Ref.PCon.TMov.StartEmergencyBrake();
        }
    }

    //  Engine Level
    private void InitEngineLevel()
    {
        engineLevelSegments = new List<Image>();
        for (int i = 0; i < 5; i++)
        {
            GameObject engineSegment = Instantiate((GameObject)Resources.Load("EngineLevelSegment"));
            engineSegment.transform.SetParent(_engineLevelLayoutGroup.transform);
            engineSegment.transform.localScale = Vector3.one;
            Image engineSegmentImg = engineSegment.GetComponent<Image>();
            engineLevelSegments.Add(engineSegmentImg);
        }
    }
    public void InitSliders()
    {
        _currentSpeedSlider.value = Ref.PCon.TMov.currentSpeed;
        _desiredSpeedSlider.value = Ref.PCon.TMov.currentSpeed;
        _currentSpeedSlider.maxValue = Ref.PCon.TMov.maxSpeed;
        _desiredSpeedSlider.maxValue = Ref.PCon.TMov.maxSpeed;
    }
    public void UpdateEngineLevel(int level, int maxLevel)
    {
        //  Just in case of syncing issues, make sure we have a bar to update
        if (engineLevelSegments.Count == 0) InitEngineLevel();

        //  Init Empty

        for (int i = 0; i < 5; i++)
        {
            Image engineSegmentImg = engineLevelSegments[i];
            if (i == 0) engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Left_Empty", typeof(Sprite)) as Sprite;
            else if (i == 4) engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Right_Empty", typeof(Sprite)) as Sprite;
            else engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Middle_Empty", typeof(Sprite)) as Sprite;
        }

        //   Update the full ones

        for (int i = 0; i < level; i++)
        {
            Image engineSegmentImg = engineLevelSegments[i];
            if (i == 0) engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Left_Full", typeof(Sprite)) as Sprite;
            else if (i == 4) engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Right_Full", typeof(Sprite)) as Sprite;
            else engineSegmentImg.sprite = Resources.Load("Art/UI/Engine Level/Engine_Level_Middle_Full", typeof(Sprite)) as Sprite;
        }
        PlayerTankMovement tmov = Ref.PCon.TMov;
        float multiplier = 1 + (level / (float)maxLevel);
        tmov.maxSpeed = tmov.baseMaxSpeed * multiplier;
        tmov.acceleration = tmov.baseAcceleration * multiplier;
        tmov.deceleration = tmov.baseDeceleration * multiplier;

        InitSliders();
    }
    public void SpeedSliderUpdated()
    {
        Ref.PCon.TMov.cruiseModeOn = true;
        if (_emergencyBrakeToggle.isOn) ActivateEmergencyBrake(false);
    }
    public void UnmatchSpeedUI()
    {
        _unmatchSpeedButton.gameObject.SetActive(false);
        Ref.PCon.TMov.enemyToMatch.GetComponent<EnemyTankController>().enemyUI.MatchSpeed(Ref.PCon.TMov.enemyToMatch, false);
    }
    public void TurnOnCruiseUI(bool on)
    {
        if (on)_cruiseButton.targetGraphic.GetComponent<Image>().sprite = Resources.Load("Art/UI/speed_control_cruise_on", typeof(Sprite)) as Sprite;
        else _cruiseButton.targetGraphic.GetComponent<Image>().sprite = Resources.Load("Art/UI/speed_control_cruise_off", typeof(Sprite)) as Sprite;
        StartStopEngineSound(on);
    }
    public void StartStopEngineSound(bool engineStarting, float level = 1)
    {
        _engineStartingSound.volume = level;
        _engineStoppingSound.volume = level;
        if (engineStarting)
        {
            _engineIdleSound.gameObject.SetActive(true);

            if (!Ref.PCon.TMov.cruiseModeOn && !engineSoundOn)
            {
                _engineStartingSound.Play();
            }
        }
        else
        {
            if(!Ref.PCon.TMov.cruiseModeOn) _engineIdleSound.gameObject.SetActive(false);
            if (engineSoundOn)
            {
                _engineStoppingSound.Play();
            }
        }
        engineSoundOn = engineStarting;
    }
}