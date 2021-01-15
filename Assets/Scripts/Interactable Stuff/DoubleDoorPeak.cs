using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* When player is in a hiding spot that has two doors (wardrobe). As soon as they fully enter the hiding spot this peek object is enabled.
 * This object is directly in front of the player inside of the closed wardrobe.
 * When player interacts, the two doors rotate open slightly which allows the player to peek out from the hiding spot.
 * On pressing the "key to leave", this script calls the interact method from the hiding spot script, which triggers the movement to leave.    
 * 
 * 
 * 
 * Fix .. looking away while peeking - it stays open.
 * keywasheld on looking still makes it peek
 */

public class DoubleDoorPeak : PlayerInteractableObject, iInteractable
{
    public bool IsInteractable { get { return true; } set { _IsInteractable = value; } }
    public event Action InteractedEvent;

    [SerializeField] private bool PlayerIsPeeking;
    //private bool PlayerIsPeeking
    //{
    //    get { return _playerIsPeeking; }
    //    set
    //    {
    //        _playerIsPeeking = value;
    //        if (_playerIsPeeking)
    //            HidingSpot.IsInHiding = false;
    //        else
    //            HidingSpot.IsInHiding = true;
    //    }       
    //}

    [Header("Door Rotation Speeds")]
    [SerializeField] private float doorPeekSpeed;

    [Header("Door Rotations When Fully Peeking")]
    [SerializeField] Vector3 door1TargetRotation;
    [SerializeField] Vector3 door2TargetRotation;

    [Header("Leaving")]
    [SerializeField] DoubleDoorWardrobeHidingSpot hidingSpot;
    [SerializeField] KeyCode keyToLeave;

    [Header("UI Sprites")]
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    //Start.
    public override void Awake() => base.Awake();
    public override void Start() => base.Start();

    void Update()
    {
        if (!PlayerIsPeeking)
        {
            //If doors aren't fully shut, rotate to close.
            hidingSpot.CloseDoorsOverTime(doorPeekSpeed);

            if (Input.GetKeyDown(keyToLeave) && IsPlayerLookingAtMe()) //Only can leave when the player isn't peeking.
            {
                hidingSpot.PlayerInteracted();
                gameObject.SetActive(false);
                AimDotUI.Instance.ChangeAimDotBackToNormal();
                UIManager.Instance.doubleInteractImage.Hide();
            }
        }

        if (Input.GetKeyUp(defaultKeyToInteract)) // Stops peeking;
        {
            PlayerIsPeeking = false;
            HidingSpot.IsInHiding = true;
        }
    }

    //IInteractable.
    public void PlayerInteracted()
    {
        PlayerIsPeeking = true;
        HidingSpot.IsInHiding = false;

        hidingSpot.RotateDoorsToTargetRotation(Quaternion.Euler(door1TargetRotation), Quaternion.Euler(door2TargetRotation),doorPeekSpeed);
    }

    public void PlayerLookedAtMe()
    {
        AimDotUI.Instance.ChangeAimDotToGreen();

        UIManager.Instance.doubleInteractImage.Show(leftSprite, rightSprite);
    }

    public void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();
        UIManager.Instance.doubleInteractImage.Hide();

        PlayerIsPeeking = false; //In case they looked away while peeking.
    }

    public void PlayerStoppedInteraction() { }
    public void PlayerIsLookingAtMe() { }
}
