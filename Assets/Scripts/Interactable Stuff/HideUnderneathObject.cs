using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUnderneathObject : HidingSpot, iInteractable, iHideable
{
    public bool IsInteractable { get { return IsMovingIntoPosition == false; } set { } }

    public event Action InteractedEvent;

    //If there are multiple hiding spots near each other (example under table and can leave through multiple sides), 
    //need to have a reference to them to change their key to interact (for the purpose of leaving)
    [Header("Other Hiding Spots")]
    [SerializeField] private HideUnderneathObject[] otherHidingSpots;

    public override void Awake()
    {
        base.Awake();
    }

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
        AimDotUI.Instance.ChangeAimDotToGreen();

        if (LeanTween.isTweening(interactImage.gameObject))
            LeanTween.cancel(interactImage.gameObject);

        LeanTween.move(interactImage.gameObject, posToMoveTo, imageMoveSpeed).setEase(imageMoveEase);
    }
    public void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();

        if (LeanTween.isTweening(interactImage.gameObject))
            LeanTween.cancel(interactImage.gameObject);

        LeanTween.move(interactImage.gameObject, interactImageStartingPosition, imageMoveSpeed).setEase(imageMoveEase);
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
        }
    }
    public void OnReachingHidingSpot()
    {
        playerCameraRotation.EnableRotation();
        playerCameraRotation.SetRotation(targetTransformForHiding.transform.eulerAngles.x);
        IsMovingIntoPosition = false;

        PlayerInteractRaycast.Instance.EnableCheckingForInteractables();

        //UI.
        interactImage.sprite = stopHidingSprite;
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
        }

        //UI.
        interactImage.sprite = hideSprite;
    }
}
