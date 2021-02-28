using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Padlock : PlayerInteractableObject, iInteractable
{
    PlayerMovement playerMovement;
    PlayerCameraRotation playerCameraRotation;
    PlayerInteractRaycast playerInteractRaycast;

    public bool IsInteractable { get { return true; } set { _IsInteractable = value; } }

    [Header("Status")]
    [SerializeField] private bool PlayerUnlockingMe;

    [Header("Camera Zoom In And Out")]
    [SerializeField] private Transform cameraTargetPosition;
    [SerializeField] private float timeForCameraToReachTargetPosition;
    [SerializeField] private LeanTweenType cameraTweenType;
    private Vector3 cameraPosAtStartOfInteraction;
    private Quaternion cameraRotationAtStartOfInteraction;

    [Header("Combination Wheels of Padlock")]
    [SerializeField] private List<RotatingLockCombination> rotatingLockCombinationsList = new List<RotatingLockCombination>();
    private LinkedList<RotatingLockCombination> rotatingLockCombinations = new LinkedList<RotatingLockCombination>();
    private LinkedListNode<RotatingLockCombination> currentLockCombination;

    public override void Awake()
    {
        base.Awake();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
        playerInteractRaycast = player.GetComponentInChildren<PlayerInteractRaycast>();
    }

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < rotatingLockCombinationsList.Count; i++)
        {
            rotatingLockCombinations.AddLast(rotatingLockCombinationsList[i]);
        }

        currentLockCombination = rotatingLockCombinations.First;
    }


    public void PlayerInteracted()
    {
        //Reset UI.
        playerInteractRaycast.DisableCheckingForInteractables();
        UIManager.Instance.aimDot.Reset();

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
        PlayerUnlockingMe = false;
        playerMovement.EnableMovement();
        playerCameraRotation.EnableRotation();
        playerInteractRaycast.EnableCheckingForInteractables();

        string combinationString = string.Empty;
        for (int i = 0; i < rotatingLockCombinationsList.Count; i++)
        {
            combinationString += $"{rotatingLockCombinationsList[i].CurrentNumber} - ";
        }
        print(combinationString);
    }

    private IEnumerator CheckForPlayerUnlockingInput()
    {
        print("checking for unlocking");
        while (true)
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


            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                break;
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
        currentLockCombination = currentLockCombination.Previous;
        if (currentLockCombination == null)
            currentLockCombination = rotatingLockCombinations.Last;
    }
    private void GoDownCombination()
    {
        currentLockCombination = currentLockCombination.Next;
        if (currentLockCombination == null)
            currentLockCombination = rotatingLockCombinations.First;
    }
}
