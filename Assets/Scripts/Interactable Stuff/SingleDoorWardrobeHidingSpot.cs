using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDoorWardrobeHidingSpot : HidingSpot, iInteractable, iHideable
{
    public bool IsInteractable { get { return (IsMovingIntoPosition == false) && (IsInHiding == false); } set { } }

    [Header("Door")]
    [SerializeField] private DoorHandle doorHandle;
    [SerializeField] private GameObject doorGameObject;
    private Rigidbody doorRigidBody;

    [Header("Door Rotation")] //Fully open / closed.
    [SerializeField] float doorRotationSpeed;
    [SerializeField] private Vector3 doorOpenRotation;
    private Quaternion doorClosedRotation;

    public override void Awake()
    {
        base.Awake();

        doorRigidBody = doorGameObject.GetComponent<Rigidbody>();
    }
    public override void Start()
    {
        base.Start();

        doorClosedRotation = doorGameObject.transform.rotation;
    }

    //IHideable.
    public void OnEnteringHidingSpot() => StartCoroutine(EnteredHidingSpot());
    private IEnumerator EnteredHidingSpot()
    {
        yield return StartCoroutine(Utility.RotateGameObjectToTarget(doorGameObject,doorClosedRotation,doorRotationSpeed));
        OnReachingHidingSpot();
    }
    public void OnLeavingHidingSpot()
    {
        
    }
    public void OnLeftHidingSpot()
    {
        playerCameraRotation.SetRotation(targetTransformOnLeaving.transform.eulerAngles.x);
        playerCameraRotation.EnableRotation();
        playerMovement.EnableMovement();
        IsMovingIntoPosition = false;
        IsInHiding = false;
    }
    public void OnReachingHidingSpot()
    {
        playerCameraRotation.SetRotation(targetTransformForHiding.transform.eulerAngles.x);

        IsInHiding = true;
        IsMovingIntoPosition = false;
        playerCharacterController.detectCollisions = true;
        playerCameraRotation.EnableRotation();

        UIManager.Instance.singleInteractImage.Show(stopHidingSprite);

        StartCoroutine(CheckForPlayerLeaving());
    }

    private IEnumerator CheckForPlayerLeaving()
    {
        while (true)
        {
            if(Input.GetKeyDown(keyToLeaveHidingSpot))
            {
                StartCoroutine(LeaveHidingSpot());
                break;
            }

            yield return null;
        }
    }

    private IEnumerator LeaveHidingSpot()
    {
        IsMovingIntoPosition = true;

        UIManager.Instance.singleInteractImage.Hide();

        playerCameraRotation.DisableRotation();

        MoveAndRotateToHidingPosition();
        yield return StartCoroutine(Utility.RotateGameObjectToTarget(doorGameObject, Quaternion.Euler(doorOpenRotation), doorRotationSpeed));

        MoveAndRotateToLeavingPosition().setOnComplete(delegate ()
        {
            OnLeftHidingSpot();
            StartCoroutine(CloseDoorAtEnd());
        });
    }

    private IEnumerator CloseDoorAtEnd()
    {
        yield return StartCoroutine(Utility.RotateGameObjectToTarget(doorGameObject, doorClosedRotation, doorRotationSpeed));
        doorRigidBody.isKinematic = false;
        doorHandle.gameObject.SetActive(true);
    }

    //IInteractable.
    public void PlayerInteracted()
    {
        if (IsInteractable)
        {
            UIManager.Instance.aimDot.Reset();

            doorRigidBody.isKinematic = true;

            playerCharacterController.detectCollisions = false;

            IsMovingIntoPosition = true;

            doorHandle.gameObject.SetActive(false);

            playerMovement.DisableMovement();
            playerCameraRotation.DisableRotation();

            MoveAndRotateToFirstPosition().setOnComplete(delegate ()
            {
                MoveAndRotateToHidingPosition().setOnComplete(OnEnteringHidingSpot);
            });
        }
    }

    protected override LTDescr MoveAndRotateToFirstPosition()
    {
        StartCoroutine(Utility.RotateGameObjectToTarget(doorGameObject,Quaternion.Euler(doorOpenRotation), doorRotationSpeed));
        return base.MoveAndRotateToFirstPosition();
    }

    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
        {
            UIManager.Instance.aimDot.ChangeToGreen();
            UIManager.Instance.singleInteractImage.Show(hideSprite);
        }
    }
    public void PlayerLookedAwayFromMe()
    {
        if(!IsInHiding)
        {
           UIManager.Instance.aimDot.Reset();
           UIManager.Instance.singleInteractImage.Hide();
        }
    }
    public void PlayerStoppedInteraction()
    {
       
    }

}
