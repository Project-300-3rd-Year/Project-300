using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : PlayerInteractableObject, iInteractable
{
    private PlayerMovement playerMovement;
    private PlayerCameraRotation playerCameraRotation;
    private CharacterController playerCharacterController;

    [SerializeField] private Transform transformToMoveIntoOnInteraction;
    [SerializeField] private Transform hidingTransform;
    [SerializeField] private Transform transformToMoveOutOf;
    [SerializeField] private float playerHeightTargetOnInteraction;

    public static bool IsInHiding;

    [SerializeField] private bool IsMovingIntoPosition;

    [SerializeField] private HidingSpot[] otherHidingSpots;


    public bool IsInteractable { get { return IsMovingIntoPosition == false; } set { } }

    public event Action InteractedEvent;

    public override void Awake()
    {
        base.Awake();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
        playerCharacterController = player.GetComponent<CharacterController>();
        //LeanTween.value(playerCharacterController.height, playerHeightTargetOnInteraction, 2f);
    }
    public override void Start() => base.Start();

    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            if(!IsInHiding) 
            {
                IsInHiding = true;
                IsMovingIntoPosition = true;

                PlayerLookedAwayFromMe();
                PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

                playerMovement.DisableMovement();
                playerCameraRotation.DisableRotation();

                for (int i = 0; i < otherHidingSpots.Length; i++)
                {
                    otherHidingSpots[i].ChangeDefaultKeyToInteract(KeyCode.Mouse1);
                }

                LeanTween.rotate(player.gameObject, transformToMoveIntoOnInteraction.transform.eulerAngles, 1.1f).setEase(LeanTweenType.easeInOutQuad);
                LeanTween.move(player.gameObject, transformToMoveIntoOnInteraction, 1.1f).setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(delegate ()
                {
                    LeanTween.move(player.gameObject, hidingTransform, 1.2f).setEase(LeanTweenType.easeInCubic);
                    LeanTween.rotate(player.gameObject, hidingTransform.transform.eulerAngles, 1.3f).setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(delegate ()
                    {
                        playerCameraRotation.EnableRotation();
                        IsMovingIntoPosition = false;

                        PlayerInteractRaycast.Instance.EnableCheckingForInteractables();
                    });
                });
            }
            else //In hiding, move away from hiding spot.
            {
                IsMovingIntoPosition = true;
                playerCameraRotation.DisableRotation();

                LeanTween.rotate(player.gameObject, hidingTransform.transform.eulerAngles, 1f)
                    .setOnComplete(delegate ()
                    {
                        LeanTween.move(player.gameObject, transformToMoveOutOf, 1f);
                        LeanTween.rotate(player.gameObject, transformToMoveOutOf.transform.eulerAngles, 1f)
                        .setOnComplete(delegate ()
                        {
                            playerCameraRotation.EnableRotation();
                            playerMovement.EnableMovement();
                            PlayerInteractRaycast.Instance.EnableCheckingForInteractables();
                            IsInHiding = false;
                            IsMovingIntoPosition = false;

                            for (int i = 0; i < otherHidingSpots.Length; i++)
                            {
                                otherHidingSpots[i].ChangeDefaultKeyToInteract(KeyCode.Mouse0);
                            }

                        });
                    });
            }
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
