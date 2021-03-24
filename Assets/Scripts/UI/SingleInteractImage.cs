using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleInteractImage : MonoBehaviour
{
    [Header("Image Component")]
    [SerializeField] private Image myImage;

    [Header("Positions")]
    [SerializeField] protected Transform targetTransform;
    protected Vector2 startingPosition;

    [Header("Movement")]
    [SerializeField] protected float timeToMoveToPosition;
    [SerializeField] protected LeanTweenType moveEase;

    // Start.
    private void Awake() { }
    private void Start()
    {
        startingPosition = transform.position;
    }

    public void Show(Sprite spriteToAssign)
    {
        myImage.sprite = spriteToAssign;

        if (LeanTween.isTweening(this.gameObject))
            LeanTween.cancel(this.gameObject);

        LeanTween.move(this.gameObject, targetTransform, timeToMoveToPosition).setEase(moveEase);

    }
    public void Hide()
    {
        if (LeanTween.isTweening(this.gameObject))
            LeanTween.cancel(this.gameObject);

        LeanTween.move(this.gameObject, startingPosition, timeToMoveToPosition).setEase(moveEase);

    }
}
