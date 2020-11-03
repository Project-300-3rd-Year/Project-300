using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInteractMessage : MonoBehaviour
{
    //Components.
    TextMeshProUGUI TMProInteractMessage;

    [Header("Colour")]
    [SerializeField] private Color interactableColour;
    [SerializeField] private Color notInteractableColour;

    //Start.
    private void Awake()
    {
        TMProInteractMessage = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        PlayerInteractRaycast.Instance.LookedAtInteractableEvent += LookedAtInteractable;
        PlayerInteractRaycast.Instance.LookedAwayFromInteractableEvent += LookedAwayFromInteractable;
    }

    private void ChangeTextAndColour(string textToDisplay, Color colourToChangeTo)
    {
        TMProInteractMessage.text = textToDisplay;
        TMProInteractMessage.color = colourToChangeTo;
    }

    //Interact raycast event subscribers.
    private void LookedAtInteractable(PlayerInteractableComponent interactableObject)
    {
        //if (interactableObject.IsInteractable)
        //    ChangeTextAndColour(interactableObject.UIMessageIfPlayerLooksAtObjectInteractable, interactableColour);
        //else
        //    ChangeTextAndColour(interactableObject.UIMessageIfPlayerLooksAtObjectNotInteractable, notInteractableColour);
    }
    private void LookedAwayFromInteractable() => ChangeTextAndColour(string.Empty, Color.white);    
}
