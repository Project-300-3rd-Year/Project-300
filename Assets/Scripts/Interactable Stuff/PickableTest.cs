using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableTest : PickableObject,iInteractable
{
    new event Action InteractedEvent;

    new bool IsInteractable
    {
        get
        {
            _IsInteractable = true;
            return _IsInteractable;
        }
        set { _IsInteractable = value; }
    }

    public override void PlayerInteracted()
    {
        base.PlayerInteracted();
    }
    new void PlayerStoppedInteraction() { }
    new void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();
    }
    public new void PlayerLookedAtMe() //MARK AS PUBLIC NEW TO CALL CORRECT METHOD.
    {
        print("is this called?");
        if (IsInteractable)
            AimDotUI.Instance.ChangeAimDotToGreen();
        else
            AimDotUI.Instance.ChangeAimDotToRed();
    }

    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public new void PlayerIsLookingAtMe()
    {
        PlayerLookedAtMe();
        print("called from correct script");
    }
}
