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
    public override void Awake() => base.Awake();
    public override void Start() => base.Start();

    //IInteractable.
    public void PlayerInteracted()
    {
        PickedUpKeyEvent?.Invoke(keyInventoryItem);
        UIManager.Instance.messageNotification.Show($"I picked up the {keyInventoryItem.keyName} key");
        PlayerLookedAwayFromMe();

        Destroy(gameObject);
    }

    public void PlayerIsLookingAtMe()
    {

    }
    public void PlayerLookedAtMe() => UIManager.Instance.aimDot.ChangeToGreen();
    public void PlayerLookedAwayFromMe() => UIManager.Instance.aimDot.Reset();
    public void PlayerStoppedInteraction()
    {
    }

}
