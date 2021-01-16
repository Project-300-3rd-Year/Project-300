using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Consider using events to call methods from UI elements. Just calling methods from singleton instance at the moment in lots of different scripts.
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

    private void Awake() { }
    private void Start() { }
}