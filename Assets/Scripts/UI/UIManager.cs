using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Script which deals with all the possible UI elements on screen during gameplay / main scene.
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
            }

            return _instance;
        }
    }

    [Header("Readable Note UI")]
    public NoteUI noteUI;

    [Header("Aim Dot At Center Of Screen")]
    public AimDot aimDot;

    [Header("Single Interact Image")]
    public SingleInteractImage singleInteractImage;

    [Header("Double Interact Image")]
    public DoubleInteractImage doubleInteractImage;

    [Header("Message Notification")] 
    public MessageNotification messageNotification;

    [Header("Pause Screen")]
    public PauseScreen pauseScreen;

    //[Header("")]

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            pauseScreen.pauseScreenDelegate?.Invoke();
        }
    }

    private void Awake() { }
    private void Start() { }
}