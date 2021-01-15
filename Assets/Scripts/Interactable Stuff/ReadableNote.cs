using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadableNote : PlayerInteractableObject,iInteractable
{
    //Components.
    PlayerMovement playerMovement;
    PlayerCameraRotation playerCameraRotation;

    [SerializeField] Note note;

    //UI.
    [SerializeField] Image imgBackground;
    [SerializeField] Image imgNote;
    [SerializeField] TextMeshProUGUI tmProDate;
    [SerializeField] TextMeshProUGUI tmProNote;

    private Coroutine readingNoteCoroutine;

    public bool IsInteractable { get { return true; } set { _IsInteractable = value;} }

    public event Action InteractedEvent;

    public override void Awake()
    {
        base.Awake();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
    }
    public override void Start()
    {
        base.Start();
    }

    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            if (readingNoteCoroutine != null)
                StopCoroutine(readingNoteCoroutine);

            readingNoteCoroutine = StartCoroutine(ReadingNoteCoroutine());

            PlayerInteractRaycast.Instance.DisableInteractionWithObjects();
            PlayerInteractRaycast.Instance.DisableCheckingForInteractables();

            Cursor.lockState = CursorLockMode.None;

            playerMovement.DisableMovement();
            playerCameraRotation.DisableRotation();

            imgBackground.gameObject.SetActive(true);
            imgNote.gameObject.SetActive(true);
            tmProDate.text = note.date;
            tmProNote.text = note.text;

        }
    }

    public void PlayerIsLookingAtMe()
    {

    }

    public void PlayerLookedAtMe()
    {
        UIManager.Instance.aimDot.ChangeToGreen();
    }

    public void PlayerLookedAwayFromMe()
    {
        UIManager.Instance.aimDot.Reset();
    }

    public void PlayerStoppedInteraction()
    {

    }

    private IEnumerator ReadingNoteCoroutine()
    {
        while (true)
        {
            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                PlayerInteractRaycast.Instance.EnableInteractionWithObjects();
                PlayerInteractRaycast.Instance.EnableCheckingForInteractables();

                Cursor.lockState = CursorLockMode.Locked;

                playerMovement.EnableMovement();
                playerCameraRotation.EnableRotation();

                imgBackground.gameObject.SetActive(false);
                imgNote.gameObject.SetActive(false);

                break;
            }

            yield return null;
        }
    }
}
