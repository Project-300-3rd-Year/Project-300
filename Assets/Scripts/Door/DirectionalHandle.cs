﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handle which can move an object in any desired direction up to a certain amount (clamping).
 * Works for a pullable drawer, sliding door etc.
 * 
 * 
 */

public class DirectionalHandle : Handle, iInteractable, iLockable
{
    public event Action InteractedEvent;
    public bool IsInteractable
    {
        get
        {
            return IsLocked == false && PlayerInteracting == false;
        }
        set
        {
            _IsInteractable = value;
        }
    }

    [Header("Locking")]
    [SerializeField] private bool _isLocked;
    [SerializeField] private KeyInventoryItem _keyToUnlockMe;
    [SerializeField] private KeyCode _keyCodeToUnlockMe;
    [SerializeField] private Sprite _unlockSprite;

    public bool IsLocked { get { return _isLocked; } set { _isLocked = value; } }
    public KeyInventoryItem KeyToUnlockMe { get { return _keyToUnlockMe; } }
    public KeyCode KeyCodeToUnlockMe { get { return _keyCodeToUnlockMe; } }
    public Sprite UnlockSprite { get { return _unlockSprite; } }


    [SerializeField] private Vector3 moveDirection;
    private Vector3 closedPosition;

    [Header("Object Move Clamping")]
    [Range(0,3)]
    [SerializeField] private float maxAmountToMoveObject;

    private float clampMinX;
    private float clampMinY;
    private float clampMinZ;
    private float clampMaxX;
    private float clampMaxY;
    private float clampMaxZ;

    // Start.
    public override void Awake() => base.Awake();
    public override void Start()
    {
        base.Start();
        interactableArea.PlayerLeftArea += PlayerStoppedInteraction;
        closedPosition = gameObjectToAffect.transform.localPosition;

        if (IsLocked)
            LockMe();

        //Improve later.
        if (moveDirection.x < 0)
        {
            clampMinX = closedPosition.x - maxAmountToMoveObject;
            clampMaxX = closedPosition.x;
        }
        else 
        {
            clampMinX = closedPosition.x;
            clampMaxX = closedPosition.x + maxAmountToMoveObject;
        }
        if (moveDirection.y < 0)
        {
            clampMinY = closedPosition.y - maxAmountToMoveObject;
            clampMaxY = closedPosition.y;
        }
        else 
        {
            clampMinY = closedPosition.y;
            clampMaxY = closedPosition.y + maxAmountToMoveObject;
        }
        if (moveDirection.z < 0)
        {
            clampMinZ = closedPosition.z - maxAmountToMoveObject;
            clampMaxZ = closedPosition.z;
        }
        else 
        {
            clampMinZ = closedPosition.z;
            clampMaxZ = closedPosition.z + maxAmountToMoveObject;
        }

        moveDirection = moveDirection.normalized;
    }

    public void UnlockMe()
    {
        IsLocked = false;
        ChangeKeyInteractCondition(holdToInteract: true);
        currentKeyToInteract = defaultKeyToInteract;
    }

    public void LockMe()
    {
        IsLocked = true;
        ChangeKeyInteractCondition(holdToInteract: false);
        currentKeyToInteract = KeyCodeToUnlockMe;
    }


    public void PlayerInteracted()
    {
        if (IsInteractable)
        {
             StartCoroutine(InteractWithHandle());
        }
        else
        {
            if (player.GetComponent<PlayerInventory>().HasKeyInInventory(KeyToUnlockMe)) //Unlock.
            {
                UnlockMe();

                UIManager.Instance.aimDot.ChangeToGreen();
                UIManager.Instance.singleInteractImage.Hide();
            }
            else
            {
                UIManager.Instance.messageNotification.Show($"It's locked... seems like I need the {KeyToUnlockMe.keyName} key...");
            }
        }
    }

    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
            UIManager.Instance.aimDot.ChangeToGreen();
        else
        {
            UIManager.Instance.aimDot.ChangeToRed();
            UIManager.Instance.singleInteractImage.Show(UnlockSprite);
        }
    }
    public void PlayerLookedAwayFromMe()
    {
        if (PlayerInteracting == false)
            UIManager.Instance.aimDot.Reset();

        if (IsLocked)
            UIManager.Instance.singleInteractImage.Hide();
    }

    public void PlayerStoppedInteraction()
    {
        if (PlayerInteracting)
        {
            PlayerInteracting = false;
            UIManager.Instance.aimDot.EnableAimDot();
            UIManager.Instance.aimDot.Reset();

            playerCameraRotation.EnableRotation();

            PlayerInteractRaycast.Instance.EnableCheckingForInteractables();
        }
    }

    private IEnumerator InteractWithHandle()
    {
        PlayerInteracting = true;

        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();
        UIManager.Instance.aimDot.DisableAimDot();

        playerCameraRotation.DisableRotation();

        Vector3 playerRelativePosition = playerRelativePositionChecker.transform.InverseTransformPoint(player.transform.position);
        Vector3 pullableObjectPositionAtStartOfInteraction = gameObjectToAffect.transform.position;

        while (inputDelegate(defaultKeyToInteract))
        {
            float desiredMouseInput = Mathf.Abs(Input.GetAxisRaw("Mouse X")) > Mathf.Abs(Input.GetAxisRaw("Mouse Y")) ? Input.GetAxisRaw("Mouse X") : Input.GetAxisRaw("Mouse Y"); //Choose input based on which left / right input is bigger.

            Vector3 pullVector = moveDirection * affectSpeed * (desiredMouseInput = playerRelativePosition.z < 0 ? desiredMouseInput : -desiredMouseInput) * Time.deltaTime;
            gameObjectToAffect.transform.Translate(pullVector);
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(gameObjectToAffect.transform.localPosition.x, clampMinX, clampMaxX),
                Mathf.Clamp(gameObjectToAffect.transform.localPosition.y, clampMinY, clampMaxY),
                Mathf.Clamp(gameObjectToAffect.transform.localPosition.z, clampMinZ, clampMaxZ));


            gameObjectToAffect.transform.localPosition = clampedPosition;

            yield return null;
        }

        PlayerStoppedInteraction();
    }
}
