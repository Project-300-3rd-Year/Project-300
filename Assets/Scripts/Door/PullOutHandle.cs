using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Fix for later - make handle a base script and door handle and this script inherit from it. Too much repeated code.
 * Code is really bad - only works if object faces certain way. FIX LATER.
 * */

public class PullOutHandle : Handle, iInteractable, iLockable
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
    public bool IsLocked { get { return _isLocked; } set { _isLocked = value; } }

    [SerializeField] private KeyInventoryItem _keyToUnlockMe;
    public KeyInventoryItem KeyToUnlockMe { get { return _keyToUnlockMe; } }

    [SerializeField] private KeyCode _keyCodeToUnlockMe;
    public KeyCode KeyCodeToUnlockMe { get { return _keyCodeToUnlockMe; } }

    [SerializeField] private Sprite _unlockSprite;
    public Sprite UnlockSprite { get { return _unlockSprite; } }

    [SerializeField] Vector3 pullDirection;
    private Vector3 closedPosition;

    [Header("Pull Object Clamping")]
    [Range(0,3)]
    [SerializeField] private float amountToPullObject;

    // Start.
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
        interactableArea.PlayerLeftArea += PlayerStoppedInteraction;
        closedPosition = gameObjectToAffect.transform.localPosition;
        print("Starting / closed position: " + closedPosition);

        if (IsLocked)
            LockMe();
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
            if (interactCoroutine == null)
                interactCoroutine = StartCoroutine(InteractWithHandle());
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
    public void PlayerIsLookingAtMe() { }
    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
        {
            UIManager.Instance.aimDot.ChangeToGreen();
        }
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

            if (interactCoroutine != null)
            {
                StopCoroutine(interactCoroutine);
                interactCoroutine = null;
            }
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

            Vector3 pullVector = pullDirection * affectSpeed * (desiredMouseInput = playerRelativePosition.z > 0 ? desiredMouseInput : -desiredMouseInput) * Time.deltaTime;
            gameObjectToAffect.transform.Translate(pullVector);
            Vector3 clampedVector = new Vector3(gameObjectToAffect.transform.localPosition.x, gameObjectToAffect.transform.localPosition.y, Mathf.Clamp(gameObjectToAffect.transform.localPosition.z, closedPosition.z - amountToPullObject, closedPosition.z));
            gameObjectToAffect.transform.localPosition = clampedVector;
            yield return null;
        }

        PlayerStoppedInteraction();
    }
}
