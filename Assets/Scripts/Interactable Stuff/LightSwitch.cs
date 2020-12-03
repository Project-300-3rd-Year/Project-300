using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : PlayerInteractableObject, iInteractable
{
    public bool IsInteractable {
        get
        {
            return true;
        }
        set
        {
            _IsInteractable = value;
        }
    }

    public event Action InteractedEvent;

    [Header("Status")]
    [SerializeField] private bool SwitchedOn;

    [Header("Light")]
    [SerializeField] private Light lightSource;

    //Start.
    public override void Awake() => base.Awake();
    public override void Start() => base.Start();

    private void TurnOnLight() => lightSource.enabled = true;
    private void TurnOffLight() => lightSource.enabled = false;

    public void PlayerInteracted()
    {
        if (SwitchedOn)
        {
            TurnOffLight();
            SwitchedOn = false;
        }
        else
        {
            TurnOnLight();
            SwitchedOn = true;
        }
    }

    public void PlayerIsLookingAtMe()
    {
      
    }

    public void PlayerLookedAtMe()
    {
        AimDotUI.Instance.ChangeAimDotToGreen();
    }

    public void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();
    }

    public void PlayerStoppedInteraction()
    {
        
    }
}
