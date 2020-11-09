using System;
using System.Collections;
using UnityEngine;

/* Something can be picked up if its not moving (rigidbody's velocity is zero) and the bool canbepickedup is true.
 * On interaction, a coroutine starts which deals with players input and the possible things the player can do while the object is in their hands.
 * While in hands, object can be thrown, rotated or dropped.
 * If player rotates too fast the object is dropped. 
 * Pickable objects do not collide with player.
 * 
 * FUTURE FIXES - 
 * Throw force doesn't take into account players current speed - so if player is moving forward the throw force isn't adjusted and doesn't travel as far.
 * If in players hands and the object gets into contact with player - it makes the player move a lot. With bigger objects this is a problem.
 * Fix for the moment is making the character controller capsule smaller.
 * Pickup "transform" position should adjust depending on object size (maybe).  
 */

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : PlayerInteractableObject,iInteractable
{
    public static event Action PlayerPickedUpObject; //Current subscribers - raycast script to enables/disable checking for interactables.
    public static event Action PlayerDroppedObject;
    public event Action InteractedEvent;

    //Components.
    Rigidbody rigidbody;
    PlayerMovement playerMovement;
    PlayerCameraRotation playerCameraRotation;

    Coroutine HandleInputCoroutine;

    [Header("Status")]
    [SerializeField] private bool InPlayersHands;
    [SerializeField] private bool IsBeingRotated;
    [SerializeField] private bool CanBePickedUp;
    [SerializeField] private bool ReachedPickupPosition;

    [Header("Speed")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    [Header("Throwing")]
    [SerializeField] private float throwForce;

    [Header("Dropping")]
    [SerializeField] private float playerHorizontalRotationSpeedToDrop; //This could be a static var for all pickable objects - not sure yet.
    [SerializeField] private float playerVerticalRotationSpeedToDrop; //This could be a static var for all pickable objects - not sure yet.

    [Header("Position To Move To")]
    [SerializeField] private Transform targetTransform;

    private float horizontalMouseRotationInput { get { return Input.GetAxisRaw("Mouse X"); } }
    private float verticalMouseRotationInput { get { return Input.GetAxisRaw("Mouse Y"); } }

    public bool IsInteractable
    {
        get
        {
            _IsInteractable = rigidbody.velocity == Vector3.zero && CanBePickedUp;
            return _IsInteractable;
        }
        set
        {
            _IsInteractable = value;
        }
    }

    public override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
    }

    public override void Start()
    {
        base.Start();
    }

    public virtual void PlayerInteracted()
    {
        if (IsInteractable)
        {
            if (HandleInputCoroutine != null)
                StopCoroutine(HandleInputCoroutine);

            HandleInputCoroutine = StartCoroutine(HandlePlayerInputInPlayerHands());
        }
    }

    private IEnumerator HandlePlayerInputInPlayerHands()
    {
        PlayerPickedMeUp();

        while (Input.GetKey(defaultKeyToInteract))
        {
            //Move towards target and check if it reached the position.
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.transform.position, moveSpeed + playerMovement.Speed);
            if (!ReachedPickupPosition)
            {
                if (Vector3.Distance(transform.position, targetTransform.transform.position) <= 0.2f)
                {
                    ReachedPickupPosition = true;
                }
            }

            //Drop from hands if player rotated too fast.
            if (playerCameraRotation.currentHorizontalRotationSpeed >= playerHorizontalRotationSpeedToDrop || playerCameraRotation.currentVerticalRotationSpeed >= playerVerticalRotationSpeedToDrop)
            {
                DropFromPlayersHands();
            }

            //Enter Rotation State.
            if (Input.GetKeyDown(KeyCode.R))
            {
                EnterRotationState();
            }

            if (Input.GetKey(KeyCode.R))
            {
                RotateOnPlayerInput();
            }

            //Leave Rotation State.
            if (Input.GetKeyUp(KeyCode.R))
            {
                LeaveRotationState();
            }

            //Throw Object.
            if (Input.GetKeyDown(KeyCode.Mouse1) && !IsBeingRotated)
            {
                ThrowMe();
            }

            yield return null;
        }

        DropFromPlayersHands();
    }

    private void DropFromPlayersHands()
    {
        PlayerDroppedObject?.Invoke();

        if (HandleInputCoroutine != null)
            StopCoroutine(HandleInputCoroutine);

        rigidbody.useGravity = true;
        rigidbody.freezeRotation = false;
        rigidbody.isKinematic = false;

        InPlayersHands = false;
        IsBeingRotated = false;
        ReachedPickupPosition = false;

        playerCameraRotation.EnableRotation();
    }

    public void PlayerPickedMeUp()
    {
        PlayerPickedUpObject?.Invoke();

        rigidbody.useGravity = false;
        rigidbody.freezeRotation = true;
        rigidbody.isKinematic = true;

        InPlayersHands = true;
    }
    private void EnterRotationState()
    {
        IsBeingRotated = true;
        playerCameraRotation.DisableRotation();
        rigidbody.freezeRotation = false;
    }
    private void LeaveRotationState()
    {
        playerCameraRotation.EnableRotation();
        IsBeingRotated = false;
    }
    private void RotateOnPlayerInput()
    {
        transform.Rotate(Camera.main.transform.up, -Mathf.Deg2Rad * horizontalMouseRotationInput * rotateSpeed, Space.World);
        transform.Rotate(Camera.main.transform.right, -Mathf.Deg2Rad * -verticalMouseRotationInput * rotateSpeed, Space.World);
    }
    private void ThrowMe()
    {
        DropFromPlayersHands();
        Vector3 forceToThrowAt = (transform.position - player.transform.position).normalized * throwForce;
        rigidbody.AddForce(forceToThrowAt, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (InPlayersHands && collision.gameObject.tag != "Ground" && !ReachedPickupPosition) //Maybe change this to check layer of ground instead.
        {
            DropFromPlayersHands();
        }
    }

    public void PlayerStoppedInteraction()
    {
        
    }

    public void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();
    }

    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
            AimDotUI.Instance.ChangeAimDotToGreen();
        else
            AimDotUI.Instance.ChangeAimDotBackToNormal();
    }

    public void PlayerIsLookingAtMe()
    {
        PlayerLookedAtMe();
    }
}
