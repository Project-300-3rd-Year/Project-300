using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Key object that player interacts with / picks up.
 * Each key object has a reference to the KeyInventory scriptable object which represents the actual key.
 * When picking up a key - event is fired which playerinventory and UI Message Notification listens out for.
 */

public class Key : PlayerInteractableObject, iInteractable
{
    public bool IsInteractable { get { return true; } set { _IsInteractable = value; } }
    public event Action<KeyInventoryItem> PickedUpKeyEvent;

    [SerializeField] private KeyInventoryItem keyInventoryItem; //Scriptable object that key represents.

    // Start.
    public override void Awake() => base.Awake();
    public override void Start() => base.Start();

    //IInteractable.
    public void PlayerInteracted()
    {
        //UIManager.Instance.messageNotification.Show($"I picked up the {keyInventoryItem.keyName} key");
        PickedUpKeyEvent?.Invoke(keyInventoryItem);
        PlayerLookedAwayFromMe();
        Destroy(gameObject);
    }

    public void PlayerLookedAtMe() => UIManager.Instance.aimDot.ChangeToGreen();
    public void PlayerLookedAwayFromMe() => UIManager.Instance.aimDot.Reset();
    public void PlayerStoppedInteraction()
    {
    }

}
