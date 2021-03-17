using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings 
{
    public static float defaultVolume = 10f;
    public static float volume;
    public static float defaultCameraSensitivity = 3f;
    public static float cameraSensitivity;

    public static void UpdateVolumeSettings(float newVolume)
    {
        volume = newVolume;
        PlayerPrefs.SetFloat("Volume", newVolume);
    }

    public static void UpdateCameraSensitivity(float newSensitivity)
    {
        cameraSensitivity = newSensitivity;
        PlayerPrefs.SetFloat("CameraSensitivity", newSensitivity);
    }

    public static void UpdateSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
            volume = PlayerPrefs.GetFloat("Volume");
        else
            volume = defaultVolume;


        if (PlayerPrefs.HasKey("CameraSensitivity"))
            cameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity");
        else
            cameraSensitivity = defaultCameraSensitivity;
    }
}
