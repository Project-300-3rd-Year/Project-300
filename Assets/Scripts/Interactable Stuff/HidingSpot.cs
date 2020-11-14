using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidingSpot : PlayerInteractableObject, iInteractable
{
    //Events.
    public event Action InteractedEvent;

    //Components.
    private PlayerMovement playerMovement;
    private PlayerCameraRotation playerCameraRotation;
    private CharacterController playerCharacterController;

    public bool IsInteractable { get { return IsMovingIntoPosition == false; } set { } }

    [Header("Hiding Status")]
    [SerializeField] private bool IsMovingIntoPosition;
    public static bool IsInHiding; //Hiding status across all hiding spots.

    [Header("First target position")]
    [SerializeField] private Transform targetTransformOnInteraction;
    [SerializeField] private float timeToMoveToFirstPos;
    [SerializeField] private float timeToRotateToFirstPos;
    [SerializeField] private LeanTweenType firstPosMoveEaseType;
    [SerializeField] private LeanTweenType firstPosRotateEaseType;

    [Header("Hiding spot")]
    [SerializeField] private Transform targetTransformForHiding;
    [SerializeField] private float timeToMoveToHidingPos;
    [SerializeField] private float timeToRotateToHidingPos;
    [SerializeField] private LeanTweenType hidingPosMoveEaseType;
    [SerializeField] private LeanTweenType hidingPosRotateEaseType;

    [Header("Leaving hiding spot")]
    [SerializeField] private Transform targetTransformOnLeaving;
    [SerializeField] private float timeToMoveToLeavingPos;
    [SerializeField] private float timeToRotateToLeavingPos;
    [SerializeField] private LeanTweenType leavingPosMoveEaseType;
    [SerializeField] private LeanTweenType leavingPosRotateEaseType;
    [SerializeField] private KeyCode keyToLeaveHidingSpot;

    [Header("Other Hiding Spots")] //If there are multiple hiding spots near each other, need to have a reference to them to change their key to interact (for the purpose of leaving)
    [SerializeField] private HidingSpot[] otherHidingSpots;

    [Header("UI")]
    [SerializeField] private Sprite hideSprite;
    [SerializeField] private Sprite stopHidingSprite;
    [SerializeField] private Image interactImage;
    [SerializeField] private Transform posToMoveTo;
    [SerializeField] private float imageMoveSpeed;
    [SerializeField] private LeanTweenType imageMoveEase;
    private Vector2 interactImageStartingPosition;

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
        interactImageStartingPosition = interactImage.transform.position;
    }

    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            if(!IsInHiding) //Not in hiding, move into hiding spot.
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

    //Moving to desired locations (returns LTDescr so can call OnComplete after using these methods).
    private LTDescr MoveToFirstPosition()
    {
        LeanTween.rotate(player.gameObject, targetTransformOnInteraction.transform.eulerAngles, timeToRotateToFirstPos).setEase(firstPosRotateEaseType);
        LeanTween.rotate(Camera.main.gameObject, targetTransformOnInteraction.transform.eulerAngles, timeToRotateToFirstPos).setEase(firstPosRotateEaseType);
        return LeanTween.move(player.gameObject, targetTransformOnInteraction, timeToMoveToFirstPos).setEase(firstPosMoveEaseType);
    }
    private LTDescr MoveToHidingPosition()
    {
        LeanTween.rotate(player.gameObject, targetTransformForHiding.transform.eulerAngles, timeToRotateToHidingPos).setEase(hidingPosRotateEaseType);
        LeanTween.rotate(Camera.main.gameObject, targetTransformForHiding.transform.eulerAngles, timeToRotateToHidingPos).setEase(hidingPosRotateEaseType);
        return LeanTween.move(player.gameObject, targetTransformForHiding, timeToMoveToHidingPos).setEase(hidingPosMoveEaseType);
    }
    private LTDescr MoveToLeavingPosition()
    {
        LeanTween.rotate(player.gameObject, targetTransformOnLeaving.transform.eulerAngles, timeToRotateToLeavingPos).setEase(leavingPosRotateEaseType);
        LeanTween.rotate(Camera.main.gameObject, targetTransformOnLeaving.transform.eulerAngles, timeToRotateToLeavingPos).setEase(leavingPosRotateEaseType);
        return LeanTween.move(player.gameObject, targetTransformOnLeaving, timeToMoveToLeavingPos).setEase(leavingPosMoveEaseType);
    }

    //Things that happen when 
    private void OnEnteringHidingSpot()
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
    private void OnReachingHidingSpot()
    {
        playerCameraRotation.EnableRotation();
        playerCameraRotation.SetRotation(targetTransformForHiding.transform.eulerAngles.x);
        IsMovingIntoPosition = false;

        PlayerInteractRaycast.Instance.EnableCheckingForInteractables();
    }
    private void OnLeavingHidingSpot()
    {
        IsMovingIntoPosition = true;

        PlayerLookedAwayFromMe();
        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

        playerCameraRotation.DisableRotation();
    }
    private void OnLeftHidingSpot()
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

    }

    public void PlayerIsLookingAtMe()
    {

    }

    public void PlayerLookedAtMe()
    {
        AimDotUI.Instance.ChangeAimDotToGreen();
        interactImage.sprite = IsInHiding == false ? hideSprite : stopHidingSprite;

        if(LeanTween.isTweening(interactImage.gameObject))
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
