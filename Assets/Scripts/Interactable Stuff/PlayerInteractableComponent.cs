using System;
using UnityEngine;

public class PlayerInteractableComponent : MonoBehaviour
{
    //Components.
    protected GameObject player;

    [SerializeField] protected bool _IsInteractable;

    [Header("Conditions For Interaction")]
    [SerializeField] public KeyCode defaultKeyToInteract;
    [SerializeField] public bool holdToInteract;
    public bool KeyWasHeldOnLookingAtMe;
    public Func<KeyCode, bool> inputDelegate;

    public virtual void Awake() => player = GameObject.FindGameObjectWithTag("Player");
    public virtual void Start() => ChangeKeyInteractCondition(holdToInteract);

    protected void ChangeKeyInteractCondition(bool holdToInteract)
    {
        if (holdToInteract)
            inputDelegate = Input.GetKey;
        else
            inputDelegate = Input.GetKeyDown;

        this.holdToInteract = holdToInteract;
    }

    protected void ChangeDefaultKeyToInteract(KeyCode newKeyCode) => defaultKeyToInteract = newKeyCode;

    //OLD

    //public virtual void PlayerInteracted()
    //{
    //    if(IsInteractable)
    //    {
    //        if (raiseInteractedEvent)
    //        {
    //            PlayerInteractedEvent?.Invoke(this);
    //            print("EVENT RAISED - INTERACTION");
    //            raiseInteractedEvent = false;
    //        }

    //        if (holdToInteract)
    //            PlayerIsInteracting = true;
    //    }
    //}

    //public virtual void PlayerStoppedInteraction()
    //{
    //    if (holdToInteract)
    //    {
    //        PlayerIsInteracting = false;
    //        KeyWasHeldOnLookingAtMe = false;
    //        print("EVENT RAISED - STOPPED INTERACTION");
    //        PlayerStoppedInteractionEvent?.Invoke(this);
    //    }

    //    raiseInteractedEvent = true;
    //}
}
