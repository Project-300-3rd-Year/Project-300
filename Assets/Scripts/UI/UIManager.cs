using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    //Notification message on bottom of screen. 
    //Interact image at top. 
    //Double interact image at top.
    //Aim dot at center.


    [Header("Single Interact Image")]
    public SingleInteractImage singleInteractImage;

    [Header("Double Interact Image")]
    public DoubleInteractImage doubleInteractImage;

    [Header("Message Notification")] 
    public MessageNotification messageNotification;

    private void Awake() { }
    private void Start()
    {
    }
}