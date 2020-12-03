using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Fix for later - make handle a base script and door handle and this script inherit from it. Too much repeated code.
 * Code is really bad - only works if object faces certain way. FIX LATER.
 * */

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

    [SerializeField] Vector3 pullDirection;
    private Vector3 closedPosition;

    [Header("Pull Object Clamping")]
    [Range(-1,0)]
    [SerializeField] private float clampAmount;


    public void PlayerInteracted()
    {
        if (PlayerInteracting == false)
        {
            if (playerInteractingCoroutine == null)
                playerInteractingCoroutine = StartCoroutine(InteractWithDoorHandle());
        }
    }

    public void PlayerIsLookingAtMe()
    {
     
    }

    public void PlayerLookedAtMe()
    {
        if (PlayerInteracting == false)
            AimDotUI.Instance.ChangeAimDotToGreen(); //Check if can open door - some other criteria maybe - has key etc.
    }

    public void PlayerLookedAwayFromMe()
    {
        if (PlayerInteracting == false)
            AimDotUI.Instance.ChangeAimDotBackToNormal();
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
        closedPosition = pullGameObject.transform.position;
    }

    public override void Awake()
    {
        base.Awake();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
    }

    private IEnumerator InteractWithDoorHandle()
    {
        Vector3 playerRelativePosition = playerRelativePositionChecker.transform.InverseTransformPoint(player.transform.position);
        PlayerInteracting = true;

        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

        playerCameraRotation.DisableRotation();
        AimDotUI.Instance.DisableAimDot();

        Vector3 pullableObjectPositionAtStartOfInteraction = pullGameObject.transform.position;

        while (inputDelegate(defaultKeyToInteract))
        {
            float desiredMouseInput = Mathf.Abs(Input.GetAxisRaw("Mouse X")) > Mathf.Abs(Input.GetAxisRaw("Mouse Y")) ? Input.GetAxisRaw("Mouse X") : Input.GetAxisRaw("Mouse Y"); //Choose input based on which left / right input is bigger.

            Vector3 pullVector = pullDirection * pullSpeed * (desiredMouseInput = playerRelativePosition.z > 0 ? desiredMouseInput : -desiredMouseInput) * Time.deltaTime;
            pullGameObject.transform.Translate(pullVector);
            Vector3 clampedVector = new Vector3(pullGameObject.transform.position.x, pullGameObject.transform.position.y, Mathf.Clamp(pullGameObject.transform.position.z, closedPosition.z + (clampAmount), closedPosition.z));
            pullGameObject.transform.position = clampedVector;
            yield return null;
        }

        PlayerStoppedInteraction();
    }
}
