using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectTestInterface : PlayerInteractableObject,iInteractable
{
    [SerializeField] private float charge;

    public bool IsInteractable
    {   get
        {
            _IsInteractable = charge > 0;
            return _IsInteractable; 
        }
        set
        {
            _IsInteractable = value;
        }
    }

    public event Action InteractedEvent;


    public override void Awake() => base.Awake();
    public override void Start()
    {
        base.Start();
        charge = 100;
    }

    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            print("interacted with CUBE");
            InteractedEvent?.Invoke();
            charge -= 0.05f;

            if (charge <= 0)
                charge = 0;
        }
    }

    public void PlayerIsLookingAtMe()
    {
        PlayerLookedAtMe();
    }

    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
            AimDotUI.Instance.ChangeAimDotToGreen();
        else
            AimDotUI.Instance.ChangeAimDotToRed();
    }

    public void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();
    }

    public void PlayerStoppedInteraction()
    {
    }
}
