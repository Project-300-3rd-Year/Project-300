using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings 
{
    public static float defaultVolume = 50f;
    public static float maxVolume = 100f;
    public static float volume;
    public static float defaultCameraSensitivity = 3f;
    public static float maxCameraSensitivity = 10f;
    public static float cameraSensitivity;


    //Update setting property and player prefs value.
    public static void UpdateVolumeSettings(float newVolume)
    {
        volume = newVolume;
        SaveVolumeSettingsToPlayerPrefs();
    }

    public static void UpdateCameraSensitivity(float newSensitivity)
    {
        cameraSensitivity = newSensitivity;
        SaveCameraSensitivitySettingsToPlayerPrefs();
    }

    public static void UpdateSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
            volume = PlayerPrefs.GetFloat("Volume");
        else
            volume = defaultVolume;

        UpdateAudioListener(volume);

        if (PlayerPrefs.HasKey("CameraSensitivity"))
            cameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity");
        else
            cameraSensitivity = defaultCameraSensitivity;
    }

    public static void SaveVolumeSettingsToPlayerPrefs() => PlayerPrefs.SetFloat("Volume", volume);
    public static void SaveCameraSensitivitySettingsToPlayerPrefs() => PlayerPrefs.SetFloat("CameraSensitivity", cameraSensitivity);

    //Only works this way when max volume is 100.
    //If we wanted maxvolume to be other than this number, would have to change this method a bit. 
    public static void UpdateAudioListener(float volume)
    {
        AudioListener.volume = volume * 0.01f;
    }
}
