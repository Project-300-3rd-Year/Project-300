using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimDotUI : MonoBehaviour
{
    public Image interactImage; //Disabled at start by default.
    private RectTransform myRectTransform;
    private Image aimDotImage;

    //Instance.
    private static AimDotUI _instance;
    public static AimDotUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AimDotUI>();
            }

            return _instance;
        }
    }

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
    public void ChangeAimDotToGreen()
    {
        LeanTween.scale(myRectTransform, sizeLookingAtInteractable, timeToChangeSize);
        LeanTween.value(gameObject, a => aimDotImage.color = a, aimDotImage.color, colourLookingAtInteractable, timeToChangeColour);
    }

    public void ChangeAimDotToRed()
    {
        LeanTween.scale(myRectTransform, defaultSize, timeToChangeSize);
        LeanTween.value(gameObject, a => aimDotImage.color = a, aimDotImage.color, colourLookingAtNotInteractable, timeToChangeColour);
    }

    public void ChangeAimDotBackToNormal()
    {
        DisableInteractImage();
        EnableAimDot();

        LeanTween.scale(myRectTransform, defaultSize, timeToChangeSize);
        LeanTween.value(gameObject, a => aimDotImage.color = a, aimDotImage.color, defaultColour, timeToChangeColour);
    }

    public void EnableAimDot() => aimDotImage.gameObject.SetActive(true);
    public void DisableAimDot() => aimDotImage.gameObject.SetActive(false);

    //INTERACT IMAGE.
    public void SetInteractImage(Sprite image)
    {
        if(interactImage.gameObject.activeSelf == false)
        {
            DisableAimDot();
            interactImage.sprite = image;
            interactImage.gameObject.SetActive(true);
        }
    }

    public void EnableInteractImage() => interactImage.gameObject.SetActive(true);
    public void DisableInteractImage() => interactImage.gameObject.SetActive(false);



    //private void ChangeOnLookedAtInteractable(Sprite image)
    //{
    //    //if (playerInteractableObject.IsInteractable)
    //    //{
    //    //    EITHER DISPLAY AN UI IMAGE THAT THE INTERACTABLE OBJECT WANTS TO SHOW OR JUST CHANGE THE AIM DOT.
    //    //    if (playerInteractableObject.UIImageToShowIfPlayerLooksAtMe != null)
    //    //    {
    //    //        interactImage.gameObject.SetActive(true);
    //    //        interactImage.sprite = playerInteractableObject.UIImageToShowIfPlayerLooksAtMe;
    //    //    }
    //    //    else
    //    //    {
    //    //        LeanTween.scale(myRectTransform, sizeLookingAtInteractable, timeToChangeSize);
    //    //        LeanTween.value(gameObject, a => myImage.color = a, myImage.color, colourLookingAtInteractable, timeToChangeColour);
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    LeanTween.scale(myRectTransform, defaultSize, timeToChangeSize);
    //    //    LeanTween.value(gameObject, a => myImage.color = a, myImage.color, colourLookingAtNotInteractable, timeToChangeColour);
    //    //}
    //}
}


//Old change colour and size using coroutine instead of leantween.
/*
private IEnumerator ChangeColourAndSizeOverTime(Color targetColour, Vector2 targetSize)
{
    float interpolation = 0;
    Vector2 currentSize = myRectTransform.sizeDelta;
    Color currentColour = myImage.color;

    while (interpolation <= 1)
    {
        interpolation += Time.deltaTime * changeSpeed;

        myImage.color = Color.Lerp(currentColour, targetColour, interpolation);
        myRectTransform.sizeDelta = Vector2.Lerp(currentSize, targetSize, interpolation);

        yield return null;
    }
}
private void ChangeOnLookedAtInteractable(PlayerInteractableObject playerInteractableObject) => changeSizeCoroutine = StartCoroutine(ChangeColourAndSizeOverTime(colourLookingAtInteractable, sizeLookingAtInteractable));
private void ChangeOnLookedAtNotInteractable(PlayerInteractableObject playerInteractableObject) => changeSizeCoroutine = StartCoroutine(ChangeColourAndSizeOverTime(colourLookingAtNotInteractable, defaultSize));
private void ChangeOnLookedAwayFromInteractable() => changeSizeCoroutine = StartCoroutine(ChangeColourAndSizeOverTime(defaultColour, defaultSize));

*/
