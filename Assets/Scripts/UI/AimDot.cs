using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimDot : MonoBehaviour
{
    private RectTransform myRectTransform;
    private Image aimDotImage;

    //AIM DOT PROPERTIES.
    [Header("Colour")]
    [SerializeField] private Color colourLookingAtInteractable;
    [SerializeField] private Color colourLookingAtNotInteractable;
    private Color defaultColour;

    [Header("Size")]
    [SerializeField] private Vector2 sizeLookingAtInteractable;
    private Vector3 defaultSize;

    [Header("Speed of change")]
    [SerializeField] private float timeToChangeSize;
    [SerializeField] private float timeToChangeColour;

    //Start.
    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
        aimDotImage = GetComponent<Image>();
    }

    private void Start()
    {
        defaultColour = aimDotImage.color;
        defaultSize = new Vector3(myRectTransform.localScale.x, myRectTransform.localScale.y, myRectTransform.localScale.z);
    }

    //AIM DOT.
    public void ChangeToGreen()
    {
        LeanTween.scale(myRectTransform, sizeLookingAtInteractable, timeToChangeSize);
        LeanTween.value(gameObject, a => aimDotImage.color = a, aimDotImage.color, colourLookingAtInteractable, timeToChangeColour);
    }

    public void ChangeToRed()
    {
        LeanTween.scale(myRectTransform, defaultSize, timeToChangeSize);
        LeanTween.value(gameObject, a => aimDotImage.color = a, aimDotImage.color, colourLookingAtNotInteractable, timeToChangeColour);
    }

    public void Reset()
    {
        LeanTween.scale(myRectTransform, defaultSize, timeToChangeSize);
        LeanTween.value(gameObject, a => aimDotImage.color = a, aimDotImage.color, defaultColour, timeToChangeColour);
    }

    public void EnableAimDot() => aimDotImage.gameObject.SetActive(true);
    public void DisableAimDot() => aimDotImage.gameObject.SetActive(false);
}
