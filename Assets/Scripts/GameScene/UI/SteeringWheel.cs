﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteeringWheel : MonoBehaviour
{
    public Transform steeringWheelParent;
    public RectTransform _steeringWheelRect;
    public GameObject _steeringWheelObject;
    public GameObject _steeringWheelPointer;
    public GameObject _steeringWheelBG;
    public GameObject _steeringWheelPrompt;
    private bool steeringWheelOpen;

    [Space(30)]
    public Button _rotateBackButton;
    public Animator _rotateBackButtonAnimator;
    private float minHoldTime = 0.75f;
    private float holdTime = 0;
    public Transform _chainParent;
    private void Awake()
    {
        _rotateBackButton.onClick.AddListener(() => REF.PCon.TRot.GetComponent<PlayerTankRotation>().TurnTankUp());
    }
    private void Start()
    {
        steeringWheelOpen = false;
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            ToggleSteeringWheel();
        }
        if (Input.GetKey(KeyCode.R))
        {
            holdTime += Time.deltaTime;
        }
        else
        {
            holdTime = 0;
            ResetSteeringWheel();
            _rotateBackButton.gameObject.SetActive(true);
        }
    }
    private void FixedUpdate()
    {
        if (holdTime >= minHoldTime)
        {
            SteeringWheelTrackMouse();
        }
        else
        {
            ResetSteeringWheel();
        }
    }

    private void ToggleSteeringWheel()
    {
        if (steeringWheelOpen) CloseSteeringWheel();
        else OpenSteeringWheel();
    }
    private void CloseSteeringWheel()
    {
        steeringWheelOpen = false;
        _steeringWheelBG.GetComponent<RectTransform>().anchoredPosition = _steeringWheelRect.anchoredPosition = Vector3.zero;
        _steeringWheelRect.SetParent(steeringWheelParent);
        _steeringWheelPrompt.SetActive(true);
    }
    private void OpenSteeringWheel()
    {
        steeringWheelOpen = true;
        _steeringWheelBG.GetComponent<RectTransform>().anchoredPosition = _steeringWheelRect.anchoredPosition = new Vector3(0, 128, 0);
        _steeringWheelRect.SetParent(steeringWheelParent);
        _steeringWheelPrompt.SetActive(true);
    }
    private void SteeringWheelTrackMouse()
    {
        _steeringWheelBG.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 128, 0);
        Vector3 startPos = new Vector3(0, 128, 0);
        Vector3 endPos;
        if (!Input.GetKey(KeyCode.Mouse0)) endPos = REF.mouse._cursorTransform.anchoredPosition;
        else endPos = _steeringWheelObject.GetComponent<RectTransform>().anchoredPosition;
        Sprite sp = Resources.Load("Art/Weapons/chain_link", typeof(Sprite)) as Sprite;

        DottedLine.DottedLine.Instance.DrawDottedUILine(startPos, endPos, Color.white, sp);

        _rotateBackButton.gameObject.SetActive(false);
        _steeringWheelPrompt.SetActive(false);

        if (Input.GetKey(KeyCode.Mouse0)) _steeringWheelRect.transform.SetParent(transform, true);
        else _steeringWheelRect.transform.SetParent(REF.mouse.mouseGameObject.transform, false);
        if (Input.GetKeyUp(KeyCode.Mouse0)) holdTime = 0;
    }
    private void ResetSteeringWheel()
    {
        if (steeringWheelOpen) OpenSteeringWheel();
        else CloseSteeringWheel();
    }

    //  RotateBack

    public void RotateFlash()
    {
        _rotateBackButtonAnimator.SetBool("Flash", true);
    }
    public void RotateIdle()
    {
        _rotateBackButtonAnimator.SetBool("Flash", false);
    }
}
