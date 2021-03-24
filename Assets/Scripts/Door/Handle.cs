using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base script for a handle that could be on a door, pullable drawer, slideable door etc.

public class Handle : PlayerInteractableObject
{
    protected PlayerCameraRotation playerCameraRotation;
    [SerializeField] protected PlayerInteractableArea interactableArea;

    public static bool PlayerInteracting; 
    protected Coroutine interactCoroutine;

    [Header("Object To Affect")]
    [SerializeField] protected GameObject gameObjectToAffect;
    [SerializeField] protected float affectSpeed;
    [SerializeField] protected Transform playerRelativePositionChecker;

    public override void Awake()
    {
        base.Awake();
        playerCameraRotation = player.GetComponentInChildren<PlayerCameraRotation>();
    }

    public override void Start() => base.Start();
}
