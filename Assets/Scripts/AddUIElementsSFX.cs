using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddUIElementsSFX : MonoBehaviour
{ 
    private Button _buttonToAddSFXTo;
    private Toggle _toggleToAddSFXTo;
    private static float pitchVariance;

    private void Awake()
    {
        pitchVariance = 0.1f;
        _buttonToAddSFXTo = GetComponent<Button>();
        _toggleToAddSFXTo = GetComponent<Toggle>();

        //pointer enter sfx
        EventTrigger.Entry pointerEnterEvent = new EventTrigger.Entry();
        pointerEnterEvent.eventID = EventTriggerType.PointerEnter;
        pointerEnterEvent.callback.AddListener((eventData) => { PlayHoverSoundEffect(); });

        if (_buttonToAddSFXTo)
        {
            //on click sfx
            _buttonToAddSFXTo.onClick.AddListener(() => PlayClickSoundEffect());
            if(!_buttonToAddSFXTo.GetComponent<EventTrigger>()) _buttonToAddSFXTo.gameObject.AddComponent<EventTrigger>();
            _buttonToAddSFXTo.GetComponent<EventTrigger>().triggers.Add(pointerEnterEvent);
        }
        if(_toggleToAddSFXTo)
        {
            _toggleToAddSFXTo.onValueChanged.AddListener( delegate { PlayClickSoundEffect(); });

            if (!_toggleToAddSFXTo.GetComponent<EventTrigger>()) _toggleToAddSFXTo.gameObject.AddComponent<EventTrigger>();
            _toggleToAddSFXTo.GetComponent<EventTrigger>().triggers.Add(pointerEnterEvent);
        }
    }

    private void PlayClickSoundEffect()
    {
        AudioManager.Singleton._clickedSound.pitch = UnityEngine.Random.Range(1 - pitchVariance, 1 + pitchVariance);
        AudioManager.Singleton._clickedSound.Play();
    }
    private void PlayHoverSoundEffect()
    {
        AudioManager.Singleton._highlightedSound.pitch = UnityEngine.Random.Range(1 - pitchVariance, 1 + pitchVariance);
        AudioManager.Singleton._highlightedSound.Play();
    }
}
