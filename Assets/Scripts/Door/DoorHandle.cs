using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Encountered an issue where I couldn't interact with door handles randomly - probably as InteractingWithDoor was set to true all the time accidentally.
 * 
*/



public class DoorHandle : PlayerInteractableObject, iInteractable
{
    //Components.
    private PlayerCameraRotation playerCameraRotation;
    [SerializeField] private PlayerInteractableArea interactableArea;

    public static bool PlayerInteractingWithDoor; //Static as there was an issue with being able to interact with two hadles at once.
    private Coroutine interactWithDoorCoroutine;

    [Header("Door - Rotation")]
    [SerializeField] private GameObject doorGameObject;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform playerRelativePositionChecker;
    Rigidbody doorRigidbody;

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

    public event Action InteractedEvent;

    public override void Awake()
    {
        base.Awake();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
        doorRigidbody = doorGameObject.GetComponent<Rigidbody>();
    }

    public override void Start()
    {
        base.Start();
        interactableArea.PlayerLeftArea += PlayerStoppedInteraction;
    }

    public void PlayerInteracted()
    {
        if(PlayerInteractingWithDoor == false)
        {
            if (interactWithDoorCoroutine == null)
                interactWithDoorCoroutine = StartCoroutine(InteractWithDoorHandle());
        }
    }

    public void PlayerIsLookingAtMe()
    {
        PlayerLookedAtMe();
    }

    public void PlayerLookedAtMe()
    {
        if (PlayerInteractingWithDoor == false)
            AimDotUI.Instance.ChangeAimDotToGreen(); //Check if can open door - some other criteria maybe - has key etc.
    }

    public void PlayerLookedAwayFromMe()
    {
        if(PlayerInteractingWithDoor == false)
            AimDotUI.Instance.ChangeAimDotBackToNormal();
    }

    private IEnumerator InteractWithDoorHandle()
    {
        Vector3 playerRelativePosition = playerRelativePositionChecker.transform.InverseTransformPoint(player.transform.position);
        PlayerInteractingWithDoor = true;

        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

        playerCameraRotation.DisableRotation();
        AimDotUI.Instance.DisableAimDot();

        while (inputDelegate(defaultKeyToInteract))
        {
            float desiredMouseInput = Mathf.Abs(Input.GetAxisRaw("Mouse X")) > Mathf.Abs(Input.GetAxisRaw("Mouse Y")) ? Input.GetAxisRaw("Mouse X") : Input.GetAxisRaw("Mouse Y"); //Choose input based on which left / right input is bigger.
            doorRigidbody.AddRelativeTorque(doorGameObject.transform.up * rotationSpeed * (desiredMouseInput = playerRelativePosition.z > 0 ? desiredMouseInput : -desiredMouseInput) * Time.deltaTime, ForceMode.VelocityChange);
            yield return null;
        }

        PlayerStoppedInteraction();
    }


    public void PlayerStoppedInteraction()
    {
        if (PlayerInteractingWithDoor)
        {
            PlayerInteractingWithDoor = false;
            AimDotUI.Instance.EnableAimDot();
            AimDotUI.Instance.ChangeAimDotBackToNormal();
            playerCameraRotation.EnableRotation();

            PlayerInteractRaycast.Instance.EnableCheckingForInteractables();

            if (interactWithDoorCoroutine != null)
            {
                StopCoroutine(interactWithDoorCoroutine);
                interactWithDoorCoroutine = null;
            }
        }
    }
}
