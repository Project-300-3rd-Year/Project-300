using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Should have made seperate scripts for door handles and just regular handles for cubbards etc. but ran out of time.
*/

public class DoorHandle : Handle, iInteractable, iLockable
{
    private Rigidbody doorRigidbody;

    private BreakDownDoor breakDownDoor; //Might not have one.
    public List<GameObject> enemyPathsToActivateOnUnlocking; //Might not have any. Should have made a seperate script.

    public bool IsInteractable
    {
        get
        {
            bool returnValue = IsLocked == false && PlayerInteracting == false;

            if (breakDownDoor != null)
                returnValue = !breakDownDoor.SequenceIsActive;

            return returnValue;

        }
        set
        {
            _IsInteractable = value;
        }
    }
    public event Action<DoorHandle> UnlockEvent;

    [Header("Locking")]
    [SerializeField] private bool _isLocked;
    [SerializeField] private KeyInventoryItem _keyToUnlockMe;
    [SerializeField] private KeyCode _keyCodeToUnlockMe;
    [SerializeField] private Sprite _unlockSprite;

    public bool IsLocked { get { return _isLocked; } set { _isLocked = value; } }
    public KeyInventoryItem KeyToUnlockMe { get { return _keyToUnlockMe; } }
    public KeyCode KeyCodeToUnlockMe { get { return _keyCodeToUnlockMe; } }
    public Sprite UnlockSprite { get { return _unlockSprite; } }

    //Start.
    public override void Awake()
    {
        base.Awake();
        doorRigidbody = gameObjectToAffect.GetComponent<Rigidbody>();
        gameObjectToAffect.TryGetComponent(out breakDownDoor);
    }
    public override void Start()
    {
        base.Start();

        interactableArea.PlayerLeftArea += PlayerStoppedInteraction;

        if (IsLocked)
            Lock();
    }

    public void Unlock()
    {
        UnlockEvent?.Invoke(this);

        IsLocked = false;
        ChangeKeyInteractCondition(holdToInteract: true);
        currentKeyToInteract = defaultKeyToInteract;
        doorRigidbody.isKinematic = false;
    }

    public void Lock()
    {
        IsLocked = true;
        ChangeKeyInteractCondition(holdToInteract: false);
        currentKeyToInteract = KeyCodeToUnlockMe;
        doorRigidbody.isKinematic = true;
    }

    //IInteractable.
    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            StartCoroutine(InteractWithDoorHandle());
        }
        else
        {
            if(breakDownDoor != null && breakDownDoor.SequenceIsActive)
                UIManager.Instance.aimDot.Reset();

            else if(player.GetComponent<PlayerInventory>().HasKeyInInventory(KeyToUnlockMe)) //Unlock.
            {
                Unlock();

                UIManager.Instance.aimDot.ChangeToGreen();
                UIManager.Instance.singleInteractImage.Hide();
            }
            else
            {
                UIManager.Instance.messageNotification.Show($"Door is locked... seems like I need the {KeyToUnlockMe.keyName} key...");
            }
        }
    }

    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
        {        
            UIManager.Instance.aimDot.ChangeToGreen();
        }
        else
        {
            if(IsLocked)
            {
                UIManager.Instance.aimDot.ChangeToRed();
                UIManager.Instance.singleInteractImage.Show(UnlockSprite);
            }
        }
    }
    public void PlayerLookedAwayFromMe()
    {
        if(PlayerInteracting == false)
            UIManager.Instance.aimDot.Reset();

        if (IsLocked)
            UIManager.Instance.singleInteractImage.Hide();
    }

    //Interaction.
    private IEnumerator InteractWithDoorHandle()
    {
        Vector3 playerRelativePosition = playerRelativePositionChecker.transform.InverseTransformPoint(player.transform.position);
        PlayerInteracting = true;

        PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

        playerCameraRotation.DisableRotation();
        UIManager.Instance.aimDot.DisableAimDot();

        while (inputDelegate(defaultKeyToInteract)) 
        {
            float desiredMouseInput = Mathf.Abs(Input.GetAxisRaw("Mouse X")) > Mathf.Abs(Input.GetAxisRaw("Mouse Y")) ? Input.GetAxisRaw("Mouse X") : Input.GetAxisRaw("Mouse Y"); //Choose input based on which left / right input is bigger.
            doorRigidbody.AddRelativeTorque(gameObjectToAffect.transform.up * affectSpeed * (desiredMouseInput = playerRelativePosition.z > 0 ? desiredMouseInput : -desiredMouseInput) * Time.deltaTime, ForceMode.VelocityChange);

            yield return null;
        }

        PlayerStoppedInteraction();
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
}
