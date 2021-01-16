﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUnderneathObject : HidingSpot, iInteractable, iHideable
{
    public bool IsInteractable { get { return IsMovingIntoPosition == false; } set { } }

    //If there are multiple hiding spots near each other (example under table and can leave through multiple sides), 
    //need to have a reference to them to change their key to interact (for the purpose of leaving)
    [Header("Other Hiding Spots")]
    [SerializeField] private HideUnderneathObject[] otherHidingSpots;

    public override void Awake() => base.Awake();

    //IInteractable.
    public void PlayerInteracted()
    {
        if (IsInteractable)
        {
            if (!IsInHiding) //Not in hiding, move into hiding spot.
            {
                OnEnteringHidingSpot();

                MoveToFirstPosition().setOnComplete(delegate ()
                {
                    MoveToHidingPosition().setOnComplete(OnReachingHidingSpot);
                });
            }
            else //In hiding, move away from hiding spot.
            {
                OnLeavingHidingSpot();

                MoveToFirstPosition().setOnComplete(delegate ()
                {
                    MoveToLeavingPosition().setOnComplete(OnLeftHidingSpot);
                });
            }
        }
    }
    public void PlayerLookedAtMe()
    {
        UIManager.Instance.aimDot.ChangeToGreen();
        UIManager.Instance.singleInteractImage.Show(currentInteractSprite);
    }
    public void PlayerLookedAwayFromMe()
    {
        UIManager.Instance.aimDot.Reset();
        UIManager.Instance.singleInteractImage.Hide();
    }
    public void PlayerStoppedInteraction()
    {
        
    }
    public void PlayerIsLookingAtMe()
    {

    }

    //IHideable.
    public void OnEnteringHidingSpot()
    {
        IsInHiding = true;
        IsMovingIntoPosition = true;

        PlayerLookedAwayFromMe();
        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

        playerMovement.DisableMovement();
        playerCameraRotation.DisableRotation();

        for (int i = 0; i < otherHidingSpots.Length; i++)
        {
            otherHidingSpots[i].ChangeCurrentKeyToInteract(keyToLeaveHidingSpot);
            //otherHidingSpots[i].
        }
    }
    public void OnReachingHidingSpot()
    {
        playerCameraRotation.EnableRotation();
        playerCameraRotation.SetRotation(targetTransformForHiding.transform.eulerAngles.x);
        IsMovingIntoPosition = false;

        PlayerInteractRaycast.Instance.EnableCheckingForInteractables();

        //UI.
        for (int i = 0; i < otherHidingSpots.Length; i++)
        {
            otherHidingSpots[i].currentInteractSprite = stopHidingSprite;
        }
    }
    public void OnLeavingHidingSpot()
    {
        IsMovingIntoPosition = true;

        PlayerLookedAwayFromMe();
        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

        playerCameraRotation.DisableRotation();
    }
    public void OnLeftHidingSpot()
    {
        playerCameraRotation.SetRotation(targetTransformOnLeaving.transform.eulerAngles.x);

        playerCameraRotation.EnableRotation();
        playerMovement.EnableMovement();

        PlayerInteractRaycast.Instance.EnableCheckingForInteractables();

        IsInHiding = false;
        IsMovingIntoPosition = false;

        for (int i = 0; i < otherHidingSpots.Length; i++)
        {
            otherHidingSpots[i].ChangeCurrentKeyToInteract(defaultKeyToInteract);
            otherHidingSpots[i].currentInteractSprite = hideSprite;
        }
    }
}
