using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UI Image which can fade to black and call a method / delegate when fade is finished.
//Activated when enemy catches you or game ends.
public class FadeToBlackImage : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeToBlack(Action delegateCalledAtEnd)
    {
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeTime).setOnComplete(delegateCalledAtEnd);
    }

}
