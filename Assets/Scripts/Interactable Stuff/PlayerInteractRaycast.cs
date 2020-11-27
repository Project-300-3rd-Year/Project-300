using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractRaycast : MonoBehaviour
{
    //Components.
    PlayerMovement playerMovement;

    //Events.
    public event Action<PlayerInteractableComponent> LookedAtInteractableEvent;
    public event Action LookedAwayFromInteractableEvent;

    private bool _canInteractWithObjects = true;
    [SerializeField] private bool CanInteractWithObjects
    {
        get
        {
            return (playerMovement.IsSprinting == false) && (_canInteractWithObjects == true);
        }
        set
        {
            _canInteractWithObjects = value;
        }
    }

    //Instance.
    private static PlayerInteractRaycast _instance;
    public static PlayerInteractRaycast Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerInteractRaycast>();
            }

            return _instance;
        }
    }

    //Components.
    [SerializeField] private PlayerInteractableObject interactableObject;
    private iInteractable IinteractableObject;

    [Header("Status")]
    [SerializeField] public bool checkForInteractableObjects;

    [Header("Ray")]
    [SerializeField] private bool CheckForRaycastLeavingInteractableObject;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layersToBlockInteractRaycast;
    [SerializeField] private float interactDistance = 200;
    private Ray ray;
    private RaycastHit hitInfo;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        PickableObject.PlayerPickedUpObject += DisableCheckingForInteractables; //Consider just calling methods instead of listening for event - not sure which is better.
        PickableObject.PlayerDroppedObject += EnableCheckingForInteractables;
    }

    private void Update()
    {
        if(CanInteractWithObjects)
        {
            if (interactableObject != null)//Looking at interactable.
            {
                if (interactableObject.inputDelegate(interactableObject.currentKeyToInteract)) //Player interacts.
                {
                    IinteractableObject.PlayerInteracted();
                }

                //if (interactableObject != null) //After interacting an object might destroy itself.
                //{
                //    if (Input.GetKeyUp(interactableObject.defaultKeyToInteract))
                //    {
                //        IinteractableObject.PlayerStoppedInteraction();
                //    }
                //}
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "InteractableArea")
        {
            if (checkForInteractableObjects)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo, interactDistance, layerMask, QueryTriggerInteraction.Collide)) //Looks at interactable.
                {
                    if ((layersToBlockInteractRaycast & 1 << hitInfo.collider.gameObject.layer ) == 0) //If the hit object's layer isn't one of the layers that blocks interact raycast.
                    {                                                                                  
                        if (!CheckForRaycastLeavingInteractableObject)
                            CheckForRaycastLeavingInteractableObject = true;

                        if (interactableObject == null || hitInfo.collider.gameObject != interactableObject.gameObject)
                        {
                            if (interactableObject != null) //If player looked at another object and the raycast didn't leave any interactable - this "deselects" the old object.
                            {
                                LookedAway();
                            }

                            hitInfo.collider.gameObject.TryGetComponent<PlayerInteractableObject>(out interactableObject);
                            hitInfo.collider.gameObject.TryGetComponent<iInteractable>(out IinteractableObject);

                            if (interactableObject != null && IinteractableObject != null) //Added extra check here as I added default layer to layermask.
                            {                                                             //In order for default layer to stop raycast - not be able to pick up items through walls etc. It does mean that this get called 
                                IinteractableObject.PlayerLookedAtMe();
                                LookedAtInteractableEvent(interactableObject);
                            }
                        }

                        if (IinteractableObject != null)
                            IinteractableObject.PlayerIsLookingAtMe();
                    }
                    else //Looks away from interactable.
                    {
                        if (interactableObject != null)
                        {
                            LookedAway();
                        }
                    }
                }               
            }
        }
    }

    private void OnTriggerExit(Collider other) //If leaves interactable area but is still looking at object.
    {
        if (other.gameObject.tag == "InteractableArea")
        {
            if (interactableObject != null)
            {
                LookedAway();
            }
        }
    }

    public void LookedAway()
    {
        IinteractableObject.PlayerLookedAwayFromMe();
        LookedAwayFromInteractableEvent();
        CheckForRaycastLeavingInteractableObject = false;
        interactableObject = null;
        IinteractableObject = null;
    }

    public void EnableCheckingForInteractables() => checkForInteractableObjects = true;
    public void DisableCheckingForInteractables() => checkForInteractableObjects = false;


    public void EnableInteractionWithObjects() => CanInteractWithObjects = true;
    public void DisableInteractionWithObjects() => CanInteractWithObjects = false;
}
