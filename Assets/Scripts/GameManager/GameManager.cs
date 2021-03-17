using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton - not used much so far, just updating the settings properties.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        Settings.UpdateSettings();
    }
}
