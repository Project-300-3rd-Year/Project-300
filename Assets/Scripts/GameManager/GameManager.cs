using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton.
//Data about game session state is here in case we need it. 
//Needed some way of telling game over scene what happened if player won or lost as it displays a message that depends on this. Probably could be done a better way.

public enum GameSessonState
{
    InMainMenu,
    Loading,
    InGame,
    GameOverWin, 
    GameOverLose
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action onGameEnd;

    public GameSessonState currentGameSessionState;
    //public GameSessonState CurrentGameSessionState { get { return _currentGameSessionState; } set { _currentGameSessionState = value; } }

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
