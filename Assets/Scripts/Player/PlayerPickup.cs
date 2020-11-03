using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public event Action<PickableObject> PickedUpObject;
    public event Action DroppedObject;

    PlayerInteractRaycast playerInteractRaycast;

    public PickableObject currentObjectInHands;
    public Transform pickUpPosition;

    private Coroutine PickUpCoroutine;

    private void Awake()
    {
        playerInteractRaycast = GetComponent<PlayerInteractRaycast>();
    }
}
