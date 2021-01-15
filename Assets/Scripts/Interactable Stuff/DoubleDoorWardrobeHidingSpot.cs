using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoorWardrobeHidingSpot : HidingSpot, iInteractable,iHideable
{
    public bool IsInteractable { get { return (IsMovingIntoPosition == false) && (IsInHiding == false); } set { } }

    public event Action InteractedEvent;

    [Header("Doors")]
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    private Rigidbody leftDoorRigidBody;
    private Rigidbody rightDoorRigidBody;

    [Header("Door Rotation")] //Fully open / closed.
    [SerializeField] float doorRotationSpeed;
    [SerializeField] private Vector3 leftDoorOpenRotation; 
    [SerializeField] private Vector3 rightDoorOpenRotation;
    private Quaternion leftDoorClosedRotation;
    private Quaternion rightDoorClosedRotation;

    [SerializeField] DoubleDoorPeak doubleDoorPeak;
    [SerializeField] private DoorHandle leftDoorHandle;
    [SerializeField] private DoorHandle rightDoorHandle;

    //Start.
    public override void Awake()
    {
        base.Awake();
        playerCharacterController = player.GetComponent<CharacterController>();

        leftDoorRigidBody = leftDoor.GetComponent<Rigidbody>();
        rightDoorRigidBody = rightDoor.GetComponent<Rigidbody>();
    }
    public override void Start()
    {
        base.Start();

        leftDoorClosedRotation = leftDoor.transform.rotation; //Treats it as if doors are closed at start.
        rightDoorClosedRotation = rightDoor.transform.rotation;

        doubleDoorPeak.gameObject.SetActive(false);
    } 

    //IInteractable.
    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            AimDotUI.Instance.ChangeAimDotBackToNormal();

            leftDoorHandle.gameObject.SetActive(false);
            rightDoorHandle.gameObject.SetActive(false);

            leftDoorRigidBody.isKinematic = true;
            rightDoorRigidBody.isKinematic = true;

            playerCharacterController.detectCollisions = false;

            IsMovingIntoPosition = true;

            playerMovement.DisableMovement();
            playerCameraRotation.DisableRotation();

            MoveToFirstPosition().setOnComplete(delegate ()
            {
                MoveToHidingPosition().setOnComplete(OnEnteringHidingSpot);
            });
        }   
        else //Hiding - called from peeking script.
        {
            if(IsInHiding)
                StartCoroutine(LeaveHidingSpot());
        }
    }

    //Entering hiding spot.
    protected override LTDescr MoveToFirstPosition()
    {
        StartCoroutine(RotateDoorsToTarget(Quaternion.Euler(leftDoorOpenRotation), Quaternion.Euler(rightDoorOpenRotation), doorRotationSpeed));
        return base.MoveToFirstPosition();
    }


    //Leaving Hiding Spot.
    private IEnumerator LeaveHidingSpot()
    {
        playerCameraRotation.DisableRotation();

        MoveToHidingPosition();
        yield return StartCoroutine(RotateDoorsToTarget(Quaternion.Euler(leftDoorOpenRotation),Quaternion.Euler(rightDoorOpenRotation), doorRotationSpeed));

        MoveToLeavingPosition().setOnComplete(delegate()
        {
            OnLeftHidingSpot();
            StartCoroutine(CloseDoorsAtEnd());
        });
    }

    private IEnumerator CloseDoorsAtEnd()
    {
        yield return StartCoroutine(RotateDoorsToTarget(leftDoorClosedRotation, rightDoorClosedRotation, doorRotationSpeed));
        leftDoorRigidBody.isKinematic = false;
        rightDoorRigidBody.isKinematic = false;

        leftDoorHandle.gameObject.SetActive(true);
        rightDoorHandle.gameObject.SetActive(true);

        IsMovingIntoPosition = false;
    }

    //IHideable.
    public void OnEnteringHidingSpot() => StartCoroutine(EnteredHidingSpot());
    private IEnumerator EnteredHidingSpot()
    {
        yield return StartCoroutine(RotateDoorsToTarget(leftDoorClosedRotation,rightDoorClosedRotation, doorRotationSpeed));
        OnReachingHidingSpot();
    }
    public void OnReachingHidingSpot()
    {
        IsInHiding = true;
        playerCharacterController.detectCollisions = true;
        playerCameraRotation.EnableRotation();
        doubleDoorPeak.gameObject.SetActive(true);
    }
    public void OnLeavingHidingSpot() { }
    public void OnLeftHidingSpot()
    {
        playerCameraRotation.SetRotation(targetTransformOnLeaving.transform.eulerAngles.x);
        playerCameraRotation.EnableRotation();
        playerMovement.EnableMovement();
        IsInHiding = false;
    }

    //Door rotating.
    public void CloseDoorsOverTime(float speed) => RotateDoorsToTargetRotation(leftDoorClosedRotation,rightDoorClosedRotation,speed);
    public void RotateDoorsToTargetRotation(Quaternion door1TargetRotation, Quaternion door2TargetRotation,float speed)
    {
        if (Quaternion.Angle(leftDoor.transform.rotation, door1TargetRotation) > 0
               || Quaternion.Angle(rightDoor.transform.rotation, door2TargetRotation) > 0)
        {
            leftDoor.transform.rotation = Quaternion.RotateTowards(leftDoor.transform.rotation, door1TargetRotation, speed * Time.deltaTime);
            rightDoor.transform.rotation = Quaternion.RotateTowards(rightDoor.transform.rotation, door2TargetRotation, speed * Time.deltaTime);
        }
    }
    private IEnumerator RotateDoorsToTarget(Quaternion door1TargetRotation, Quaternion door2TargetRotation, float speed)
    {
        while (Quaternion.Angle(leftDoor.transform.rotation, door1TargetRotation) > 0
               || Quaternion.Angle(rightDoor.transform.rotation, door2TargetRotation) > 0)
        {
            leftDoor.transform.rotation = Quaternion.RotateTowards(leftDoor.transform.rotation, door1TargetRotation, speed * Time.deltaTime);
            rightDoor.transform.rotation = Quaternion.RotateTowards(rightDoor.transform.rotation, door2TargetRotation, speed * Time.deltaTime);
            yield return null;
        }
    }

    //Iinteractable.
    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
        {
            AimDotUI.Instance.ChangeAimDotToGreen();
            UIManager.Instance.singleInteractImage.Show(hideSprite);
        }
    }
    public void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();
        UIManager.Instance.singleInteractImage.Hide();
    }
    public void PlayerIsLookingAtMe() { }
    public void PlayerStoppedInteraction() { }

}
