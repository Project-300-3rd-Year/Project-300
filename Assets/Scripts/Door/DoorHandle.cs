using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
 * 
*/

public class DoorHandle : Handle, iInteractable, iLockable
{
    Rigidbody doorRigidbody;

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
    public event Action InteractedEvent;

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
            if(player.GetComponent<PlayerInventory>().HasKeyInInventory(KeyToUnlockMe)) //Unlock.
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
            UIManager.Instance.aimDot.ChangeToRed();
            UIManager.Instance.singleInteractImage.Show(UnlockSprite);
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

    public void EnemyBreakDownDoorSequence()
    {
        StartCoroutine(EnemyHittingDoorSequence());
    }

    //Leaving this here for now, when enemy ai reaches a closed door this is code that could be called to force it open.
    IEnumerator EnemyHittingDoorSequence()
    {
        //Variables that would be needed for this -
        //public AudioClip[] possibleSmashDoorSounds;
        //public AudioClip[] possibleHitDoorSounds;
        //AudioSource audioSource;

        System.Random rng = new System.Random();
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 4f, 0)), affectSpeed));
        //AudioSource.PlayClipAtPoint(possibleHitDoorSounds[rng.Next(0,possibleHitDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 0, 0)), -affectSpeed));
        yield return new WaitForSeconds(rng.Next(2, 3));
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 4f, 0)), affectSpeed));
        //AudioSource.PlayClipAtPoint(possibleHitDoorSounds[rng.Next(0, possibleHitDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 0, 0)), -affectSpeed));
        yield return new WaitForSeconds(rng.Next(2, 3));
        //AudioSource.PlayClipAtPoint(possibleSmashDoorSounds[rng.Next(0, possibleSmashDoorSounds.Length)], transform.position);
        yield return StartCoroutine(ApplyForceToDoorUntilItReachesAngle((Quaternion.Euler(0, 90f, 0)), affectSpeed));
    }

    //Make sure to apply a negative or positive force depending on which way you want the door to move.
    IEnumerator ApplyForceToDoorUntilItReachesAngle(Quaternion targetRotation, float forceToApply)
    {
        doorRigidbody.isKinematic = false;

        while (Quaternion.Angle(gameObjectToAffect.transform.localRotation, targetRotation) > 4f) //Anything less tends to overshoot if the open speed is fast.
        {
            print(Quaternion.Angle(gameObjectToAffect.transform.localRotation, targetRotation));
            doorRigidbody.AddRelativeTorque(gameObjectToAffect.transform.up * forceToApply, ForceMode.VelocityChange);
            yield return null;
        }

        doorRigidbody.isKinematic = true;
    }
}
