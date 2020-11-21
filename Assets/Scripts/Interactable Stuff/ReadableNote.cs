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
            PlayerInteractRaycast.Instance.DisableInteractionWithObjects();

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
        AimDotUI.Instance.ChangeAimDotToGreen();
    }

    public void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();
    }

    public void PlayerStoppedInteraction()
    {

    }
}
