using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubleInteractImage : MonoBehaviour
{
    [Header("Image Components")]
    [SerializeField] protected Image leftImage;
    [SerializeField] protected Image rightImage;
    [Header("Target Positions")]
    [SerializeField] protected Transform targetTransform;
    protected Vector2 startingPosition;

    [Header("Movement")]
    [SerializeField] private float timeToMoveToPosition;
    [SerializeField] private LeanTweenType moveEase;

    // Start.
    private void Awake()
    {
        
    }
    private void Start()
    {
        startingPosition = transform.position;
    }

    public void Show(Sprite leftSprite,Sprite rightSprite)
    {
        leftImage.sprite = leftSprite;
        rightImage.sprite = rightSprite;

        if (LeanTween.isTweening(this.gameObject))
        {
            LeanTween.cancel(this.gameObject);
        }

        LeanTween.move(this.gameObject, targetTransform, timeToMoveToPosition).setEase(moveEase);
    }

    public void Hide()
    {
        if (LeanTween.isTweening(this.gameObject))
        {
            LeanTween.cancel(this.gameObject);
        }

        LeanTween.move(this.gameObject, startingPosition, timeToMoveToPosition).setEase(moveEase);
    }
}
