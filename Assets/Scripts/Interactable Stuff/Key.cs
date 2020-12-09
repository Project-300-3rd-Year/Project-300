using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : PlayerInteractableObject, iInteractable
{
    public bool IsInteractable { get { return true; } set { _IsInteractable = value; } }

    public event Action InteractedEvent;
    public event Action<KeyInventoryItem> PickedUpKeyEvent;


    [SerializeField] private KeyInventoryItem keyInventoryItem; //Scriptable object that key represents.

    // Start.
    public override void Start() => base.Start();
    public override void Awake() => base.Awake();

    //IInteractable.
    public void PlayerInteracted()
    {
        PickedUpKeyEvent?.Invoke(keyInventoryItem);
        Destroy(gameObject);
    }
    public void PlayerIsLookingAtMe()
    {

    }
    public void PlayerLookedAtMe() => AimDotUI.Instance.ChangeAimDotToGreen();
    public void PlayerLookedAwayFromMe() => AimDotUI.Instance.ChangeAimDotBackToNormal();
    public void PlayerStoppedInteraction()
    {
    }

    private void OnDestroy() => PlayerLookedAwayFromMe();
}
