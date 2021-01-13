using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidingSpot : PlayerInteractableObject
{
    //Components.
    protected PlayerMovement playerMovement;
    protected PlayerCameraRotation playerCameraRotation;
    protected CharacterController playerCharacterController;

    [Header("Hiding Status")]
    [SerializeField] protected bool IsMovingIntoPosition;

    public static bool IsInHiding; //Hiding status across all hiding spots.

    [Header("First target position")]
    [SerializeField] private Transform targetTransformOnInteraction;
    [SerializeField] private float timeToMoveToFirstPos;
    [SerializeField] private float timeToRotateToFirstPos;
    [SerializeField] private LeanTweenType firstPosMoveEaseType;
    [SerializeField] private LeanTweenType firstPosRotateEaseType;

    [Header("Hiding spot")]
    [SerializeField] protected Transform targetTransformForHiding;
    [SerializeField] private float timeToMoveToHidingPos;
    [SerializeField] protected float timeToRotateToHidingPos;
    [SerializeField] private LeanTweenType hidingPosMoveEaseType;
    [SerializeField] protected LeanTweenType hidingPosRotateEaseType;

    [Header("Leaving hiding spot")]
    [SerializeField] protected Transform targetTransformOnLeaving;
    [SerializeField] private float timeToMoveToLeavingPos;
    [SerializeField] private float timeToRotateToLeavingPos;
    [SerializeField] private LeanTweenType leavingPosMoveEaseType;
    [SerializeField] private LeanTweenType leavingPosRotateEaseType;
    [SerializeField] protected KeyCode keyToLeaveHidingSpot;

    [Header("UI")]
    [SerializeField] protected Sprite hideSprite;
    [SerializeField] protected Sprite stopHidingSprite;
    protected Sprite currentInteractSprite;

    public override void Awake()
    {
        base.Awake();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
        playerCharacterController = player.GetComponent<CharacterController>();
    }
    public override void Start()
    {
        base.Start();
        currentInteractSprite = hideSprite;
    }

    //Moving to desired locations (returns LTDescr so can call OnComplete after using these methods).
    //All hiding spot types can use these methods.
    protected virtual LTDescr MoveToFirstPosition()
    {
        LeanTween.rotate(player.gameObject, targetTransformOnInteraction.transform.eulerAngles, timeToRotateToFirstPos).setEase(firstPosRotateEaseType);
        LeanTween.rotate(Camera.main.gameObject, targetTransformOnInteraction.transform.eulerAngles, timeToRotateToFirstPos).setEase(firstPosRotateEaseType);
        return LeanTween.move(player.gameObject, targetTransformOnInteraction, timeToMoveToFirstPos).setEase(firstPosMoveEaseType);
    }
    protected virtual LTDescr MoveToHidingPosition()
    {
        LeanTween.rotate(player.gameObject, targetTransformForHiding.transform.eulerAngles, timeToRotateToHidingPos).setEase(hidingPosRotateEaseType);
        LeanTween.rotate(Camera.main.gameObject, targetTransformForHiding.transform.eulerAngles, timeToRotateToHidingPos).setEase(hidingPosRotateEaseType);
        return LeanTween.move(player.gameObject, targetTransformForHiding, timeToMoveToHidingPos).setEase(hidingPosMoveEaseType);
    }
    protected virtual LTDescr MoveToLeavingPosition()
    {
        LeanTween.rotate(player.gameObject, targetTransformOnLeaving.transform.eulerAngles, timeToRotateToLeavingPos).setEase(leavingPosRotateEaseType);
        LeanTween.rotate(Camera.main.gameObject, targetTransformOnLeaving.transform.eulerAngles, timeToRotateToLeavingPos).setEase(leavingPosRotateEaseType);
        return LeanTween.move(player.gameObject, targetTransformOnLeaving, timeToMoveToLeavingPos).setEase(leavingPosMoveEaseType);
    }

    ////Old code when no inheritence - it used iinteractable as well
    //protected virtual void OnEnteringHidingSpot()
    //{
    //    IsInHiding = true;
    //    IsMovingIntoPosition = true;

    //    //PlayerLookedAwayFromMe();
    //    PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

    //    playerMovement.DisableMovement();
    //    playerCameraRotation.DisableRotation();

    //    for (int i = 0; i < otherHidingSpots.Length; i++)
    //    {
    //        otherHidingSpots[i].ChangeCurrentKeyToInteract(keyToLeaveHidingSpot);
    //    }
    //}
    //protected virtual void OnReachingHidingSpot()
    //{
    //    playerCameraRotation.EnableRotation();
    //    playerCameraRotation.SetRotation(targetTransformForHiding.transform.eulerAngles.x);
    //    IsMovingIntoPosition = false;

    //    PlayerInteractRaycast.Instance.EnableCheckingForInteractables();
    //}
    //protected virtual void OnLeavingHidingSpot()
    //{
    //    IsMovingIntoPosition = true;

    //   // PlayerLookedAwayFromMe();
    //    PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

    //    playerCameraRotation.DisableRotation();
    //}
    //protected virtual void OnLeftHidingSpot()
    //{
    //    playerCameraRotation.SetRotation(targetTransformOnLeaving.transform.eulerAngles.x);

    //    playerCameraRotation.EnableRotation();
    //    playerMovement.EnableMovement();

    //    PlayerInteractRaycast.Instance.EnableCheckingForInteractables();

    //    IsInHiding = false;
    //    IsMovingIntoPosition = false;

    //    for (int i = 0; i < otherHidingSpots.Length; i++)
    //    {
    //        otherHidingSpots[i].ChangeCurrentKeyToInteract(defaultKeyToInteract);
    //    }

    //}









    //public void PlayerInteracted()
    //{
    //    if(IsInteractable)
    //    {
    //        if(!IsInHiding) //Not in hiding, move into hiding spot.
    //        {
    //            OnEnteringHidingSpot();

    //            MoveToFirstPosition().setOnComplete(delegate ()
    //            {
    //                MoveToHidingPosition().setOnComplete(OnReachingHidingSpot);
    //            });
    //        }
    //        else //In hiding, move away from hiding spot.
    //        {
    //            OnLeavingHidingSpot();

    //            MoveToFirstPosition().setOnComplete(delegate ()
    //            {
    //                MoveToLeavingPosition().setOnComplete(OnLeftHidingSpot);
    //            });
    //        }
    //    }
    //}

    //public void PlayerIsLookingAtMe()
    //{

    //}

    //public void PlayerLookedAtMe()
    //{
    //    AimDotUI.Instance.ChangeAimDotToGreen();
    //    interactImage.sprite = IsInHiding == false ? hideSprite : stopHidingSprite;

    //    if(LeanTween.isTweening(interactImage.gameObject))
    //        LeanTween.cancel(interactImage.gameObject);

    //    LeanTween.move(interactImage.gameObject, posToMoveTo, imageMoveSpeed).setEase(imageMoveEase);
    //}

    //public void PlayerLookedAwayFromMe()
    //{
    //    AimDotUI.Instance.ChangeAimDotBackToNormal();

    //    if (LeanTween.isTweening(interactImage.gameObject))
    //        LeanTween.cancel(interactImage.gameObject);

    //    LeanTween.move(interactImage.gameObject, interactImageStartingPosition, imageMoveSpeed).setEase(imageMoveEase);
    //}

    //public void PlayerStoppedInteraction()
    //{

    //}
















    //private IEnumerator CheckInputForLeavingHidingSpot()
    //{
    //    while(IsInHiding)
    //    {
    //        if(Input.GetKeyDown(KeyCode.Mouse1))
    //        {
    //            IsMovingIntoPosition = true;
    //            playerCameraRotation.DisableRotation();

    //            LeanTween.rotate(player.gameObject, hidingTransform.transform.eulerAngles, 1f)
    //                .setOnComplete(delegate()
    //                {
    //                    LeanTween.move(player.gameObject, transformToMoveOutOf, 1f);
    //                    LeanTween.rotate(player.gameObject, transformToMoveOutOf.transform.eulerAngles, 1f)
    //                    .setOnComplete(delegate()
    //                    {
    //                        playerCameraRotation.EnableRotation();
    //                        playerMovement.EnableMovement();
    //                        PlayerInteractRaycast.Instance.EnableCheckingForInteractables();
    //                        IsInHiding = false;
    //                        IsMovingIntoPosition = false;

    //                        //IsInteractable = true;

    //                    });
    //                });
    //        }

    //        yield return null;
    //    }
    //}
}
