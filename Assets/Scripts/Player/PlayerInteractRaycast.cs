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
    public PlayerInteractableObject interactableObject;
    private iInteractable IinteractableObject;

    [Header("Status")]
    [SerializeField] public bool checkForInteractableObjects;

    [Header("Ray")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layersToBlockInteractRaycast;
    [SerializeField] private float interactDistance = 200;
    private Ray ray;
    private RaycastHit hitInfo;

    //Start.
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        //Consider just calling methods instead of listening for event - not sure which is better.
        PickableObject.PlayerPickedUpObject += DisableCheckingForInteractables; 
        PickableObject.PlayerDroppedObject += EnableCheckingForInteractables;
    }

    private void Update()
    {
        if(CanInteractWithObjects)
        {
            //Looking at interactable? Check for interaction.
            if (interactableObject != null)
            {
                if (interactableObject.inputDelegate(interactableObject.currentKeyToInteract) && interactableObject.KeyWasHeldOnLookingAtMe == false) 
                {
                    IinteractableObject.PlayerInteracted();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (checkForInteractableObjects)
        {
            if (other.gameObject.tag == "InteractableArea")   //Player enters the vicinity of an interactable object.
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out hitInfo, interactDistance, layerMask, QueryTriggerInteraction.Collide)) //Looks at interactable.
                {
                    if(HitObjectBlocksRaycast == false)
                    {
                        if (interactableObject == null || hitInfo.collider.gameObject != interactableObject.gameObject) //Looked at a new interactable object.
                        {
                            if (interactableObject != null) //If player looked at another object and the raycast didn't leave any interactable - this "deselects" the old object.
                            {
                                LookedAwayFromInteractable();
                            }

                            hitInfo.collider.gameObject.TryGetComponent<PlayerInteractableObject>(out interactableObject); //Get components from object.
                            hitInfo.collider.gameObject.TryGetComponent<iInteractable>(out IinteractableObject);

                            if (interactableObject != null && IinteractableObject != null) //If object has the components.
                            {
                                if (interactableObject.inputDelegate(interactableObject.currentKeyToInteract)) //If interact key was held at the moment the player looked at object.
                                    LookedAwayFromInteractable();                                              //Set object to null (calling "lookedaway")
                                else                                                                           //Otherwise inform the object that the player looked at it.
                                {
                                    IinteractableObject.PlayerLookedAtMe();
                                    LookedAtInteractableEvent?.Invoke(interactableObject);
                                }
                            }
                        }
                    }
                    else 
                    {
                        if (interactableObject != null)
                        {
                            LookedAwayFromInteractable();
                        }
                    }
                }
                else
                {
                    if(interactableObject != null)
                    {
                        LookedAwayFromInteractable();
                    }
                }
            }
        }
    }

    //Check if an object's layer is one of the layers that blocks the interact raycast.
    private bool HitObjectBlocksRaycast => (layersToBlockInteractRaycast & 1 << hitInfo.collider.gameObject.layer) != 0; 

    private void OnTriggerExit(Collider other) //If leaves interactable area but is still looking at object.
    {
        if (other.gameObject.tag == "InteractableArea")
        {
            if (interactableObject != null)
            {
                LookedAwayFromInteractable();
            }
        }
    }

    public void LookedAwayFromInteractable()
    {
        IinteractableObject.PlayerLookedAwayFromMe();
        LookedAwayFromInteractableEvent?.Invoke();
        interactableObject = null;
        IinteractableObject = null;
    }

    public void EnableCheckingForInteractables() => checkForInteractableObjects = true;
    public void DisableCheckingForInteractables()
    {
        checkForInteractableObjects = false;
        interactableObject = null;
        IinteractableObject = null;
    }

    public void EnableInteractionWithObjects() => CanInteractWithObjects = true;
    public void DisableInteractionWithObjects() => CanInteractWithObjects = false;
}
