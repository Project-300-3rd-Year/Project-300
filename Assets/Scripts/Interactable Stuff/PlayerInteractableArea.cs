using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractableArea : PlayerInteractableComponent 
{
    public event Action PlayerEnteredArea;
    public event Action PlayerLeftArea;


    //[Header("Player Status")]
    //[SerializeField] protected bool PlayerInRange;
    //[SerializeField] protected bool PlayerHeldInteractKeyUponEntering = false;

    //public override void Awake()
    //{
    //    base.Awake();
    //}

    //public override void Start()
    //{
    //    base.Start();
    //}

    //public virtual void Update()
    //{
    //    if(PlayerInRange)
    //    {
    //        if(PlayerHeldInteractKeyUponEntering == false)
    //        {
    //            if (inputDelegate(defaultKeyToInteract))
    //            {
    //                PlayerInteracted();
    //            }
    //        }

    //        if(Input.GetKeyUp(defaultKeyToInteract))
    //        {
    //            if (PlayerHeldInteractKeyUponEntering == false)
    //                PlayerStoppedInteraction();

    //            PlayerHeldInteractKeyUponEntering = false;
    //        }
    //    }
    //}

    //public override void PlayerInteracted()
    //{
    //    base.PlayerInteracted();
    //}

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            PlayerEnteredArea?.Invoke();

            //PlayerInRange = true;

            //if (inputDelegate(defaultKeyToInteract))
            //{
            //    PlayerHeldInteractKeyUponEntering = true;
            //}
        }
    }
    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            PlayerLeftArea?.Invoke();
            //PlayerInRange = false;
            //PlayerIsInteracting = false;
            //PlayerHeldInteractKeyUponEntering = false;
        }
    }
}
