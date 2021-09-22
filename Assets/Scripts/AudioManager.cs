using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public static float masterVolume;
    public static float musicVolume;
    public static float SFXVolume;
    public float defaultVol;

    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;

    private void Start()
    {
        defaultVol = 0.75f;
        InitVolume();
    }
    public void InitVolume()
    {
        SetMasterVolume(defaultVol);
        SetMusicVolume(defaultVol);
        SetSFXVolume(defaultVol);
    }
    public void SetMasterVolume(float Volume)
    {
        if (MasterVolumeSlider.value == 1)
        {
            audioMixer.SetFloat("MasterVolume", 0);
        }
        else if(MasterVolumeSlider.value == 0)
        {
            audioMixer.SetFloat("MasterVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Volume) * 20);
        }
        masterVolume = Volume;
        MasterVolumeSlider.value = Volume;
    }
    public void SetMusicVolume(float Volume)
    {
        if (MusicVolumeSlider.value == 1)
        {
            audioMixer.SetFloat("MusicVolume", 0);
        }
        else if (MusicVolumeSlider.value == 0)
        {
            audioMixer.SetFloat("MusicVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Volume) * 20);
        }
        musicVolume = Volume;
        MusicVolumeSlider.value = Volume;
    }
    public void SetSFXVolume(float Volume)
    {
        if (SFXVolumeSlider.value == 1)
        {
            audioMixer.SetFloat("SFXVolume", 0);
        }
        else if (SFXVolumeSlider.value == 0)
        {
            audioMixer.SetFloat("SFXVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Volume) * 20);
        }
        SFXVolume = Volume;
        SFXVolumeSlider.value = Volume;
    }
}
