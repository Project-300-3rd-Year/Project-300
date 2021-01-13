using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
            }

            return _instance;
        }
    }

    //Notification message on bottom of screen. 
    //Interact image at top. 
    //Double interact image at top.
    //Aim dot at center.


    [Header("Single Interact Image")] 
    //[SerializeField] protected Sprite interactSprite; Passed into method from elsewhere.
    [SerializeField] protected Image interactImage;
    [SerializeField] protected Transform interactImageTargetTransform;
    [SerializeField] protected float interactImageMoveSpeed;
    [SerializeField] protected LeanTweenType interactImageMoveEase;
    protected Vector2 interactImageStartingPosition;

    private void Awake()
    {

    }

    private void Start()
    {
        interactImageStartingPosition = interactImage.transform.position;
    }

    //Single Interact Image.
    public void ActivateSingleInteractImage(Sprite spriteToAssign)
    {
        interactImage.sprite = spriteToAssign;

        if (LeanTween.isTweening(interactImage.gameObject)) 
            LeanTween.cancel(interactImage.gameObject);

        LeanTween.move(interactImage.gameObject, interactImageTargetTransform, interactImageMoveSpeed).setEase(interactImageMoveEase);

    }
    public void DisableSingleInteractImage()
    {
        if (LeanTween.isTweening(interactImage.gameObject))
            LeanTween.cancel(interactImage.gameObject);

        LeanTween.move(interactImage.gameObject, interactImageStartingPosition, interactImageMoveSpeed).setEase(interactImageMoveEase);

    }
}
