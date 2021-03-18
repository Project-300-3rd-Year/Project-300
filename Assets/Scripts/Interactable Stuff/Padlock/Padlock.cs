using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Treats this as though combination wheels of padlock starts from 0.
// Created this near the end of the project and didn't have enough time to expand on it to make it better - like spawning in random places to unlock a variety of different things. 
// Player finds the combination through reading a note that is in the building. Note is always in the same place and not spawned in a random place which we would have liked to do.

[RequireComponent(typeof(Animator))]
public class Padlock : PlayerInteractableObject, iInteractable
{
    public event Action unlockedEvent;

    Animator animator;

    PlayerMovement playerMovement;
    PlayerCameraRotation playerCameraRotation;
    PlayerInteractRaycast playerInteractRaycast;

    public bool IsInteractable { get { return Unlocked == false; } set { _IsInteractable = value; } }

    [Header("Status")]
    [SerializeField] private bool Unlocked;
    [SerializeField] private bool PlayerAttemptingToUnlockMe;

    [Header("Camera Zoom In And Out")]
    [SerializeField] private Transform cameraTargetPosition;
    [SerializeField] private float timeForCameraToReachTargetPosition;
    [SerializeField] private LeanTweenType cameraTweenType;
    private Vector3 cameraPosAtStartOfInteraction;
    private Quaternion cameraRotationAtStartOfInteraction;

    [Header("Combination Wheels of Padlock")]
    [SerializeField] private List<RotatingLockCombination> rotatingLockCombinationsList = new List<RotatingLockCombination>();
    [SerializeField] private Material m_highlightLockCombination;
    private Material m_defaultLockCombination;
    private LinkedList<RotatingLockCombination> rotatingLockCombinations = new LinkedList<RotatingLockCombination>();
    private LinkedListNode<RotatingLockCombination> currentLockCombination;

    [Header("Combination To Unlock")]
    public int[] combinationToUnlock;

    //Animations.

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        playerMovement = player.GetComponent<PlayerMovement>();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
        playerInteractRaycast = player.GetComponentInChildren<PlayerInteractRaycast>();

        //Populate linked list.
        for (int i = 0; i < rotatingLockCombinationsList.Count; i++)
        {
            rotatingLockCombinations.AddLast(rotatingLockCombinationsList[i]);
        }

        currentLockCombination = rotatingLockCombinations.First;

        //Set up highlight material.
        m_defaultLockCombination = rotatingLockCombinationsList[0].GetComponent<Renderer>().material;

        //Create new random unlock combination.
        CreateRandomCombination();
    }
    public override void Start()
    {
        base.Start();        
    }

    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            //Reset UI.
            playerInteractRaycast.DisableCheckingForInteractables();
            UIManager.Instance.aimDot.Reset();
            UIManager.Instance.aimDot.DisableAimDot();

            //Lock player in place.
            playerMovement.DisableMovement();
            playerCameraRotation.DisableRotation();

            //Move Camera.
            cameraPosAtStartOfInteraction = Camera.main.transform.position;
            cameraRotationAtStartOfInteraction = Camera.main.transform.rotation;

            LeanTween.move(Camera.main.gameObject, cameraTargetPosition, timeForCameraToReachTargetPosition).setEase(cameraTweenType);
            LeanTween.rotate(Camera.main.gameObject, cameraTargetPosition.rotation.eulerAngles, timeForCameraToReachTargetPosition).setEase(cameraTweenType)
                .setOnComplete(() => StartCoroutine(CheckForPlayerUnlockingInput()));
        }
    }

    public void PlayerLookedAtMe()
    {
        if (IsInteractable)
        {
            UIManager.Instance.aimDot.ChangeToGreen();
        }
    }
    public void PlayerLookedAwayFromMe()
    {
        UIManager.Instance.aimDot.Reset();      
    }
    public void PlayerStoppedInteraction()
    {
        PlayerAttemptingToUnlockMe = false;
        playerMovement.EnableMovement();
        playerCameraRotation.EnableRotation();
        playerInteractRaycast.EnableCheckingForInteractables();
        UIManager.Instance.aimDot.EnableAimDot();

        currentLockCombination.Value.ResetMaterial();

        if(Unlocked)
        {
            unlockedEvent?.Invoke();
            Destroy(gameObject);
        }
    }

    private IEnumerator CheckForPlayerUnlockingInput()
    {
        PlayerAttemptingToUnlockMe = true;
        currentLockCombination.Value.AssignNewMaterial(m_highlightLockCombination);

        while (PlayerAttemptingToUnlockMe)
        {
            if(animator.GetBool("Idle"))
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    GoUpCombination();
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    GoDownCombination();
                }

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentLockCombination.Value.RotateLeft();

                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentLockCombination.Value.RotateRight();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AttemptToUnlockPadlock();
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    PlayerAttemptingToUnlockMe = false;
                }
            }           

            yield return null;

        }

        MoveCameraBackToPlayer();
    }

    private void MoveCameraBackToPlayer()
    {
        LeanTween.move(Camera.main.gameObject, cameraPosAtStartOfInteraction, timeForCameraToReachTargetPosition).setEase(cameraTweenType);
        LeanTween.rotate(Camera.main.gameObject, cameraRotationAtStartOfInteraction.eulerAngles, timeForCameraToReachTargetPosition).setEase(cameraTweenType)
            .setOnComplete(PlayerStoppedInteraction);
    }

    private void GoUpCombination()
    {
        currentLockCombination.Value.ResetMaterial();

        currentLockCombination = currentLockCombination.Previous;
        if (currentLockCombination == null)
            currentLockCombination = rotatingLockCombinations.Last;

        currentLockCombination.Value.AssignNewMaterial(m_highlightLockCombination);
    }
    private void GoDownCombination()
    {
        currentLockCombination.Value.ResetMaterial();

        currentLockCombination = currentLockCombination.Next;
        if (currentLockCombination == null)
            currentLockCombination = rotatingLockCombinations.First;

        currentLockCombination.Value.AssignNewMaterial(m_highlightLockCombination);
    }

    private void AttemptToUnlockPadlock()
    {
        if (IsCombinationCorrect())
        {
            Unlocked = true;
            AnimateUnlockSuccess();
        }
        else
            AnimateUnlockFailure();
    }

    private bool IsCombinationCorrect()
    {
        for (int i = 0; i < rotatingLockCombinationsList.Count; i++)
        {
            if (rotatingLockCombinationsList[i].CurrentNumber != combinationToUnlock[i])
                return false;
        }

        return true;
    }

    private void CreateRandomCombination()
    {
        System.Random rng = new System.Random();
        combinationToUnlock = new int[rotatingLockCombinationsList.Count];
        for (int i = 0; i < combinationToUnlock.Length; i++)
        {
            combinationToUnlock[i] = rng.Next(0, rotatingLockCombinationsList[i].numberAmount);
        }
    }

    public void StopPlayerUnlockInput()
    {
        PlayerAttemptingToUnlockMe = false;
    }

    //Animation.
    public void AnimateUnlockFailure()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("UnlockFailure", true);
    }
    public void AnimateIdle()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("UnlockFailure", false);
    }
    public void AnimateUnlockSuccess()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("UnlockSuccess", true);
    }

    //Debugging.
    private void PrintCurrentCombination()
    {
        string combinationString = string.Empty;
        for (int i = 0; i < rotatingLockCombinationsList.Count; i++)
        {
            combinationString += $"{rotatingLockCombinationsList[i].CurrentNumber} - ";
        }
        print(combinationString);
    }
}
