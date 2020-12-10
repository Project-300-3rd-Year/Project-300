using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public bool IsLocked { get { return _IsLocked; } set { _IsLocked = value; } }


    [Header("Status")]
    [SerializeField] private bool _IsLocked;
    public KeyInventoryItem keyToUnlockMe;
    [SerializeField] private KeyCode keyCodeToUnlockMe;

    [Header("Rotation")]
    [SerializeField] private GameObject doorGameObject;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform playerRelativePositionChecker;
    Rigidbody doorRigidbody;

    [Header("UI")] //CHANGE THIS AFTER DEMO. USE THE SAME THING AS HIDING SPOT UI.
    [SerializeField] protected Sprite unlockSprite;
    [SerializeField] protected Image unlockImage;
    [SerializeField] protected Transform posToMoveTo;
    [SerializeField] protected float imageMoveSpeed;
    [SerializeField] protected LeanTweenType imageMoveEase;
    protected Vector2 unlockImageStartingPosition;

    public bool IsInteractable
    {
        get
        {         
            return IsLocked == false && PlayerInteractingWithDoor == false;
        }
        set
        {
            _IsInteractable = value;
        }
    }

    public event Action InteractedEvent;

    //Start.
    public override void Awake()
    {
        base.Awake();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
        doorRigidbody = doorGameObject.GetComponent<Rigidbody>();
    }
    public override void Start()
    {
        base.Start();

        if(unlockImage != null)
            unlockImageStartingPosition = unlockImage.transform.position;

        interactableArea.PlayerLeftArea += PlayerStoppedInteraction;
        CheckIfIsLocked();
    }

    private void CheckIfIsLocked()
    {
        if (IsLocked)
        {
            ChangeKeyInteractCondition(holdToInteract: false);
            currentKeyToInteract = keyCodeToUnlockMe;
        }
    }

    public void UnlockDoor()
    {
        IsLocked = false;
        ChangeKeyInteractCondition(holdToInteract: true);
        currentKeyToInteract = defaultKeyToInteract;
    }

    //IInteractable.
    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            if (interactWithDoorCoroutine == null)
                interactWithDoorCoroutine = StartCoroutine(InteractWithDoorHandle());
        }
        else
        {
            if(player.GetComponent<PlayerInventory>().HasKeyInInventory(keyToUnlockMe)) //Unlock door.
            {
                UnlockDoor();

                AimDotUI.Instance.ChangeAimDotToGreen();

                if (LeanTween.isTweening(unlockImage.gameObject)) //MAKE BETTER - REALLY BAD
                    LeanTween.cancel(unlockImage.gameObject);

                LeanTween.move(unlockImage.gameObject, unlockImageStartingPosition, imageMoveSpeed).setEase(imageMoveEase);

            }
            else
            {

                MessageNotification.Instance.ActivateNotificationMessage($"Door is locked... seems like I need the {keyToUnlockMe.keyName} key...");
            }
        }
    }
    public void PlayerIsLookingAtMe()
    {

    }
    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
        {
            AimDotUI.Instance.ChangeAimDotToGreen();
        }
        else
        {
            AimDotUI.Instance.ChangeAimDotToRed();
            unlockImage.sprite = unlockSprite;

            if (LeanTween.isTweening(unlockImage.gameObject))
                LeanTween.cancel(unlockImage.gameObject);

            LeanTween.move(unlockImage.gameObject, posToMoveTo, imageMoveSpeed).setEase(imageMoveEase);
        }

        //if (player.GetComponent<PlayerInventory>().HasKeyInInventory(keyToUnlockMe) && IsLocked == true) //MAKE BETTER - REALLY BAD
        //{
        //    unlockImage.sprite = unlockSprite;

        //    if (LeanTween.isTweening(unlockImage.gameObject))
        //        LeanTween.cancel(unlockImage.gameObject);

        //    LeanTween.move(unlockImage.gameObject, posToMoveTo, imageMoveSpeed).setEase(imageMoveEase);
        //}

    }
    public void PlayerLookedAwayFromMe()
    {
        if(PlayerInteractingWithDoor == false)
            AimDotUI.Instance.ChangeAimDotBackToNormal();

        if(IsLocked)
        {
            if (LeanTween.isTweening(unlockImage.gameObject))
                LeanTween.cancel(unlockImage.gameObject);

            LeanTween.move(unlockImage.gameObject, unlockImageStartingPosition, imageMoveSpeed).setEase(imageMoveEase);
        }


        //if (player.GetComponent<PlayerInventory>().HasKeyInInventory(keyToUnlockMe)) //MAKE BETTER - REALLY BAD
        //{
        //    if (LeanTween.isTweening(unlockImage.gameObject))
        //        LeanTween.cancel(unlockImage.gameObject);

        //    LeanTween.move(unlockImage.gameObject, unlockImageStartingPosition, imageMoveSpeed).setEase(imageMoveEase);
        //}

    }

    //Interaction.
    private IEnumerator InteractWithDoorHandle()
    {
        Vector3 playerRelativePosition = playerRelativePositionChecker.transform.InverseTransformPoint(player.transform.position);
        PlayerInteractingWithDoor = true;

        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

        playerCameraRotation.DisableRotation();
        AimDotUI.Instance.DisableAimDot();

        while (inputDelegate(defaultKeyToInteract)) //Should ideally be calling the delegate in base script, but wanted to 
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
