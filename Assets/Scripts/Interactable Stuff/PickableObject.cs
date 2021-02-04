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
 * Make a variable for the magnitude of velocity that the player can pick up the item.
 * 
 */

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : PlayerInteractableObject,iInteractable
{
    public static event Action PlayerPickedUpObject; //Current subscribers - raycast script to enables/disable checking for interactables.
    public static event Action PlayerDroppedObject;

    //Components.
    Rigidbody rigidBody;
    PlayerMovement playerMovement;
    PlayerCameraRotation playerCameraRotation;

    Coroutine HandleInputCoroutine;

    [Header("Status")]
    [SerializeField] private bool InPlayersHands;
    [SerializeField] private bool IsBeingRotated;
    [SerializeField] private bool CanBePickedUp;
    //[SerializeField] private bool ReachedPickupPosition;

    [Header("Speed")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    [Header("Throwing")]
    [SerializeField] private float throwForce;

    [Header("Dropping")]
    [SerializeField] private float playerHorizontalRotationSpeedToDrop; //This could be a static var for all pickable objects - not sure yet.
    [SerializeField] private float playerVerticalRotationSpeedToDrop; //This could be a static var for all pickable objects - not sure yet.
    [SerializeField] private float collisionVelocityToDrop; 

    [Header("Position To Move To")]
    [SerializeField] private Transform targetTransform;

    private float horizontalMouseRotationInput { get { return Input.GetAxisRaw("Mouse X"); } }
    private float verticalMouseRotationInput { get { return Input.GetAxisRaw("Mouse Y"); } }

    public bool IsInteractable
    {
        get
        {
            _IsInteractable = rigidBody.velocity.magnitude <= 0.5f && CanBePickedUp;
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

        rigidBody = GetComponent<Rigidbody>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
    }

    public override void Start() => base.Start();

    public virtual void PlayerInteracted()
    {
        if (IsInteractable)
        {
            if (HandleInputCoroutine != null)
                StopCoroutine(HandleInputCoroutine);

            HandleInputCoroutine = StartCoroutine(HandleInputInPlayerHands());
        }
    }

    private IEnumerator HandleInputInPlayerHands()
    {
        PlayerPickedMeUp();

        while (inputDelegate(defaultKeyToInteract))
        {
            //Move towards target and check if it reached the position.
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.transform.position, moveSpeed + playerMovement.Speed);

            //Vector3 moveVector =  Vector3.MoveTowards(transform.position, targetTransform.transform.position, moveSpeed + playerMovement.Speed * Time.deltaTime);
            //rigidBody.MovePosition(moveVector);

            //Drop from hands if player rotated too fast.
            if(!IsBeingRotated)
            {
                if (playerCameraRotation.currentHorizontalRotationSpeed >= playerHorizontalRotationSpeedToDrop || playerCameraRotation.currentVerticalRotationSpeed >= playerVerticalRotationSpeedToDrop)
                {
                    DropFromPlayersHands();
                }
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

        rigidBody.useGravity = true;
        rigidBody.freezeRotation = false;
        //rigidbody.isKinematic = false;

        InPlayersHands = false;
        IsBeingRotated = false;
        //ReachedPickupPosition = false;

        playerCameraRotation.EnableRotation();

        //PlayerLookedAtMe();
    }

    public void PlayerPickedMeUp()
    {
        transform.position = targetTransform.position;

        PlayerPickedUpObject?.Invoke();

        rigidBody.useGravity = false;
        rigidBody.freezeRotation = true;
        //rigidbody.isKinematic = true;

        InPlayersHands = true;

        PlayerLookedAwayFromMe();
    }
    private void EnterRotationState()
    {
        IsBeingRotated = true;
        playerCameraRotation.DisableRotation();
        rigidBody.freezeRotation = false;
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
        rigidBody.AddForce(forceToThrowAt, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (InPlayersHands && collision.gameObject.tag != "Ground" && collision.gameObject.layer != LayerMask.NameToLayer("Pickable")) //Maybe change this to check layer of ground instead.
        {
            DropFromPlayersHands();
        }
    }

    public void PlayerStoppedInteraction()
    {
        
    }

    public void PlayerLookedAwayFromMe()
    {
        UIManager.Instance.aimDot.Reset();
    }

    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
            UIManager.Instance.aimDot.ChangeToGreen();
        else
            UIManager.Instance.aimDot.Reset();
    }

}
