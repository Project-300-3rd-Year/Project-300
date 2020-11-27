using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoorPeak : PlayerInteractableObject, iInteractable
{
    public bool IsInteractable { get { return true; } set { _IsInteractable = value; } }
    public event Action InteractedEvent;

    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;

    [SerializeField] Vector3 door1TargetRotation;
    [SerializeField] Vector3 door2TargetRotation;

    private Quaternion door1ClosedRotation;
    private Quaternion door2ClosedRotation;

    [SerializeField] private float doorPeekSpeed;
    [SerializeField] private bool PlayerIsPeeking;

    Coroutine handlePeekingCoroutine;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        door1ClosedRotation = door1.transform.rotation;
        door2ClosedRotation = door2.transform.rotation;
    }

    void Update()
    {
        if (!PlayerIsPeeking)
        {
            //If doors aren't fully shut, rotate to close.
            if(Quaternion.Angle(door1.transform.rotation, door1ClosedRotation) > 0 || Quaternion.Angle(door2.transform.rotation, door2ClosedRotation) > 0)
            {
                door1.transform.rotation = Quaternion.RotateTowards(door1.transform.rotation, door1ClosedRotation, doorPeekSpeed * Time.deltaTime);
                door2.transform.rotation = Quaternion.RotateTowards(door2.transform.rotation, door2ClosedRotation, doorPeekSpeed * Time.deltaTime);
            }
        }

        if (Input.GetKeyUp(defaultKeyToInteract))
        {
            PlayerIsPeeking = false;
        }
    }

    //IInteractable.
    public void PlayerInteracted()
    {
        PlayerIsPeeking = true;

        if (Quaternion.Angle(door1.transform.rotation, Quaternion.Euler(door1TargetRotation)) > 1.5f 
            || Quaternion.Angle(door2.transform.rotation, Quaternion.Euler(door2TargetRotation)) > 1.5f)
        {
            door1.transform.rotation = Quaternion.RotateTowards(door1.transform.rotation, Quaternion.Euler(door1TargetRotation), doorPeekSpeed * Time.deltaTime);
            door2.transform.rotation = Quaternion.RotateTowards(door2.transform.rotation, Quaternion.Euler(door2TargetRotation), doorPeekSpeed * Time.deltaTime);
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

        PlayerIsPeeking = false; //In case they looked away while peeking.
    }

    public void PlayerStoppedInteraction()
    {
       
    }
}
