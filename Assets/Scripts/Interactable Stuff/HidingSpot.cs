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

    [SerializeField] private bool IsInHiding;


    public bool IsInteractable { get { return true; } set { } }

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
            IsInHiding = true;

            PlayerLookedAwayFromMe();
            PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

            playerMovement.DisableMovement();
            playerCameraRotation.DisableRotation();

            LeanTween.rotate(player.gameObject, transformToMoveIntoOnInteraction.transform.eulerAngles, 1f);
            LeanTween.move(player.gameObject, transformToMoveIntoOnInteraction, 1f)
                .setOnComplete(delegate ()
                {
                   LeanTween.move(player.gameObject, hidingTransform, 1f);
                   LeanTween.rotate(player.gameObject, hidingTransform.transform.eulerAngles, 1f)
                    .setOnComplete(delegate ()
                    {
                       playerCameraRotation.EnableRotation();
                        StartCoroutine(CheckInputForLeavingHidingSpot());
                    });
                });
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

    private IEnumerator CheckInputForLeavingHidingSpot()
    {
        while(IsInHiding)
        {
            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                IsInHiding = false;

                playerCameraRotation.DisableRotation();

                LeanTween.rotate(player.gameObject, hidingTransform.transform.eulerAngles, 1f)
                    .setOnComplete(delegate()
                    {
                        LeanTween.move(player.gameObject, transformToMoveOutOf, 1f);
                        LeanTween.rotate(player.gameObject, transformToMoveOutOf.transform.eulerAngles, 1f)
                        .setOnComplete(delegate()
                        {
                            playerCameraRotation.EnableRotation();
                            playerMovement.EnableMovement();
                            PlayerInteractRaycast.Instance.EnableCheckingForInteractables();

                        });
                    });
            }

            yield return null;
        }
    }
}
