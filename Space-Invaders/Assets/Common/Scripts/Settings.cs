using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static bool UseController { get; set; }
    public static float MusicVolume { get; set; }
    public static float SFXVolume { get; set; }

    public static void Load()
    {
        UseController = (PlayerPrefs.GetInt("UseController", 1) == 1);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("UseController", (UseController) ? 1 : 0);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.Save();
    }
}
