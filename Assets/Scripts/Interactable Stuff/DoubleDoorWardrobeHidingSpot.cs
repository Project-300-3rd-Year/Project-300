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

    [Header("Door Rotation")] //Fully open / closed.
    [SerializeField] float doorRotationSpeed;
    [SerializeField] private Vector3 leftDoorOpenRotation; 
    [SerializeField] private Vector3 rightDoorOpenRotation;
    private Quaternion leftDoorClosedRotation;
    private Quaternion rightDoorClosedRotation;

    [SerializeField] DoubleDoorPeak doubleDoorPeak;

    public override void Awake()
    {
        base.Awake();
        playerCharacterController = player.GetComponent<CharacterController>();
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
        if(!IsInHiding)
        {
            playerCharacterController.detectCollisions = false;

            playerMovement.DisableMovement();
            playerCameraRotation.DisableRotation();

            MoveToFirstPosition().setOnComplete(delegate ()
            {
                MoveToHidingPosition().setOnComplete(OnEnteringHidingSpot);

            });
        }   
    }

    protected override LTDescr MoveToFirstPosition()
    {
        StartCoroutine(RotateTwoObjectsToTargetPosition(leftDoor,rightDoor,Quaternion.Euler(leftDoorOpenRotation),Quaternion.Euler(rightDoorOpenRotation),doorRotationSpeed));

        //StartCoroutine(RotateObjectToTargetRotation(leftDoor, Quaternion.Euler(leftDoorOpenRotation), doorRotationSpeed));
        //StartCoroutine(RotateObjectToTargetRotation(rightDoor, Quaternion.Euler(rightDoorOpenRotation), doorRotationSpeed));
        return base.MoveToFirstPosition();
    }

    public void PlayerIsLookingAtMe()
    {
        
    }

    public void PlayerLookedAtMe()
    {
        if(IsInteractable)
        {
            AimDotUI.Instance.ChangeAimDotToGreen();

            if (LeanTween.isTweening(interactImage.gameObject))
                LeanTween.cancel(interactImage.gameObject);

            LeanTween.move(interactImage.gameObject, posToMoveTo, imageMoveSpeed).setEase(imageMoveEase);
        }
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

    //IHideable.
    public void OnEnteringHidingSpot()
    {
        StartCoroutine(EnteredHidingSpot());
    }
    private IEnumerator EnteredHidingSpot()
    {
        yield return StartCoroutine(RotateTwoObjectsToTargetPosition(leftDoor, rightDoor, leftDoorClosedRotation,rightDoorClosedRotation, doorRotationSpeed));

        IsInHiding = true;
        playerCharacterController.detectCollisions = true;
        playerCameraRotation.EnableRotation();
        doubleDoorPeak.gameObject.SetActive(true);

        //leftDoor.gameObject.SetActive(false);
        //rightDoor.gameObject.SetActive(false);
    }


    public void OnLeavingHidingSpot()
    {
        
    }

    public void OnLeftHidingSpot()
    {
        
    }

    public void OnReachingHidingSpot()
    {
        
    }

    //private IEnumerator RotateObjectToTargetRotation(GameObject gameObject, Quaternion target, float rotationSpeed)
    //{
    //    Quaternion rotationAtStart = gameObject.transform.rotation;

    //    while (Quaternion.Angle(gameObject.transform.rotation, target) > 0)
    //    {
    //        gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, target, rotationSpeed);
    //        yield return null;
    //    }
    //}

    private IEnumerator RotateTwoObjectsToTargetPosition(GameObject gameObject1, GameObject gameObject2, Quaternion object1TargetRotation, Quaternion object2TargetRotation, float rotationSpeed)
    {
        Quaternion obj1RotationAtStart = gameObject1.transform.rotation;
        Quaternion obj2RotationAtStart = gameObject2.transform.rotation;

        while (Quaternion.Angle(gameObject1.transform.rotation, object1TargetRotation) > 0
               || Quaternion.Angle(gameObject2.transform.rotation, object2TargetRotation) > 0)
        {
            gameObject1.transform.rotation = Quaternion.RotateTowards(gameObject1.transform.rotation, object1TargetRotation, rotationSpeed);
            gameObject2.transform.rotation = Quaternion.RotateTowards(gameObject2.transform.rotation, object2TargetRotation, rotationSpeed);
            yield return null;
        }
    }

}
