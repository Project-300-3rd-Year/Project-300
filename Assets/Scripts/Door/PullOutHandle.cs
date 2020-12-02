using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullOutHandle : PlayerInteractableObject, iInteractable
{
    public bool IsInteractable
    {
        get
        {
            return _IsInteractable;
        }
        set
        {
            _IsInteractable = value;
        }
    }

    private bool PlayerInteracting;
    private Coroutine playerInteractingCoroutine;

    public event Action InteractedEvent;

    private PlayerCameraRotation playerCameraRotation;
    [SerializeField] private PlayerInteractableArea interactableArea;


    [Header("Pull Object")]
    [SerializeField] private GameObject pullGameObject;
    [SerializeField] private float pullSpeed;
    [SerializeField] private Transform playerRelativePositionChecker;

    public void PlayerInteracted()
    {
       
    }

    public void PlayerIsLookingAtMe()
    {
     
    }

    public void PlayerLookedAtMe()
    {
     
    }

    public void PlayerLookedAwayFromMe()
    {
        
    }

    public void PlayerStoppedInteraction()
    {
        if (PlayerInteracting)
        {
            PlayerInteracting = false;
            AimDotUI.Instance.EnableAimDot();
            AimDotUI.Instance.ChangeAimDotBackToNormal();
            playerCameraRotation.EnableRotation();

            PlayerInteractRaycast.Instance.EnableCheckingForInteractables();

            if (playerInteractingCoroutine != null)
            {
                StopCoroutine(playerInteractingCoroutine);
                playerInteractingCoroutine = null;
            }
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        interactableArea.PlayerLeftArea += PlayerStoppedInteraction;
    }

    public override void Awake()
    {
        base.Awake();
    }

    private IEnumerator InteractWithDoorHandle()
    {
        Vector3 playerRelativePosition = playerRelativePositionChecker.transform.InverseTransformPoint(player.transform.position);
        PlayerInteracting = true;

        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

        playerCameraRotation.DisableRotation();
        AimDotUI.Instance.DisableAimDot();

        while (inputDelegate(defaultKeyToInteract))
        {
            float desiredMouseInput = Mathf.Abs(Input.GetAxisRaw("Mouse X")) > Mathf.Abs(Input.GetAxisRaw("Mouse Y")) ? Input.GetAxisRaw("Mouse X") : Input.GetAxisRaw("Mouse Y"); //Choose input based on which left / right input is bigger.

            //doorRigidbody.AddRelativeTorque(doorGameObject.transform.up * rotationSpeed * (desiredMouseInput = playerRelativePosition.z > 0 ? desiredMouseInput : -desiredMouseInput) * Time.deltaTime, ForceMode.VelocityChange);
            yield return null;
        }

        PlayerStoppedInteraction();
    }
}
