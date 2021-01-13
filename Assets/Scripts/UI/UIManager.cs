using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 *
 * 
 *  
 *  
 */



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

    [Header("Interact Images Moving")]
    [SerializeField] protected float interactImageMoveSpeed;
    [SerializeField] protected LeanTweenType interactImageMoveEase;

    [Header("Single Interact Image")] 
    [SerializeField] protected Image singleInteractImage;
    [SerializeField] protected Transform singleInteractImageTargetTransform;
    protected Vector2 singleInteractImageStartingPosition;

    [Header("Double Interact Image")]
    [SerializeField] protected Image leftInteractImage;
    [SerializeField] protected Image rightInteractImage;
    [SerializeField] protected Transform leftInteractImageTargetTransform;
    [SerializeField] protected Transform rightInteractImageTargetTransform;
    protected Vector2 leftInteractImageStartingPosition;
    protected Vector2 rightInteractImageStartingPosition;



    private void Awake()
    {

    }

    private void Start()
    {
        singleInteractImageStartingPosition = singleInteractImage.transform.position;
        leftInteractImageStartingPosition = leftInteractImage.transform.position;
        rightInteractImageStartingPosition = rightInteractImage.transform.position;
    }

    //Single Interact Image.
    public void ActivateSingleInteractImage(Sprite spriteToAssign)
    {
        singleInteractImage.sprite = spriteToAssign;

        if (LeanTween.isTweening(singleInteractImage.gameObject)) 
            LeanTween.cancel(singleInteractImage.gameObject);

        LeanTween.move(singleInteractImage.gameObject, singleInteractImageTargetTransform, interactImageMoveSpeed).setEase(interactImageMoveEase);

    }
    public void DisableSingleInteractImage()
    {
        if (LeanTween.isTweening(singleInteractImage.gameObject))
            LeanTween.cancel(singleInteractImage.gameObject);

        LeanTween.move(singleInteractImage.gameObject, singleInteractImageStartingPosition, interactImageMoveSpeed).setEase(interactImageMoveEase);

    }

    //Double Interact Image.
    public void ActivateDoubleInteractImage(Sprite leftSprite, Sprite rightSprite)
    {
        leftInteractImage.sprite = leftSprite;
        rightInteractImage.sprite = rightSprite;

        if (LeanTween.isTweening(leftInteractImage.gameObject) || LeanTween.isTweening(rightInteractImage.gameObject))
        {
            LeanTween.cancel(leftInteractImage.gameObject);
            LeanTween.cancel(rightInteractImage.gameObject);
        }

        LeanTween.move(leftInteractImage.gameObject, leftInteractImageTargetTransform, interactImageMoveSpeed).setEase(interactImageMoveEase);
        LeanTween.move(rightInteractImage.gameObject, rightInteractImageTargetTransform, interactImageMoveSpeed).setEase(interactImageMoveEase);

    }
    public void DisableDoubleInteractImage()
    {
        if (LeanTween.isTweening(leftInteractImage.gameObject) || LeanTween.isTweening(rightInteractImage.gameObject))
        {
            LeanTween.cancel(leftInteractImage.gameObject);
            LeanTween.cancel(rightInteractImage.gameObject);
        }

        LeanTween.move(leftInteractImage.gameObject, leftInteractImageStartingPosition, interactImageMoveSpeed).setEase(interactImageMoveEase);
        LeanTween.move(rightInteractImage.gameObject, rightInteractImageStartingPosition, interactImageMoveSpeed).setEase(interactImageMoveEase);
    }
}
