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

    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;

    private void Start()
    {
        SetMasterVolume(1);
        SetMusicVolume(1);
        SetSFXVolume(1);
    }
    public void SetMasterVolume(float Volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Volume) * 20);
        MasterVolumeSlider.value = Volume;
        masterVolume = Volume;
    }
    public void SetMusicVolume(float Volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Volume) * 20);
        MusicVolumeSlider.value = musicVolume;
        musicVolume = Volume;
    }
    public void SetSFXVolume(float Volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Volume) * 20);
        SFXVolumeSlider.value = SFXVolume;
        SFXVolume = Volume;
    }
}
