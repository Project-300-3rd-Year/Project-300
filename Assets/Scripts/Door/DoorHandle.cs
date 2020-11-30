using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : PlayerInteractableObject, iInteractable
{
    //Components.
    private PlayerCameraRotation playerCameraRotation;
    [SerializeField] private PlayerInteractableArea interactableArea;

    [Header("Interaction")]
    [SerializeField] private bool PlayerInteractingWithDoor;
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
            _IsInteractable = true;
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

    void Update()
    {
       // print(playerRelativePositionChecker.transform.InverseTransformPoint(player.transform.position));
    }

    public void PlayerInteracted()
    {
        if(interactWithDoorCoroutine == null)
            interactWithDoorCoroutine = StartCoroutine(InteractWithDoorHandle());
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

    public void PlayerStoppedInteraction()
    {
        PlayerInteractingWithDoor = false;
        AimDotUI.Instance.EnableAimDot();
        AimDotUI.Instance.ChangeAimDotBackToNormal();
        playerCameraRotation.EnableRotation();

        if (interactWithDoorCoroutine != null)
        {
            StopCoroutine(interactWithDoorCoroutine);
            interactWithDoorCoroutine = null;
        }
    }

    private IEnumerator InteractWithDoorHandle()
    {
        Vector3 playerRelativePosition = playerRelativePositionChecker.transform.InverseTransformPoint(player.transform.position);

        PlayerInteractingWithDoor = true;

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

    //float ClampAngle(float angle, float from, float to)
    //{
    //    // accepts e.g. -80, 80
    //    if (angle < 0f) angle = 360 + angle;
    //    if (angle > 180f) return Mathf.Max(angle, 360 + from);
    //    return Mathf.Min(angle, to);
    //}
}
