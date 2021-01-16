using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* UI element that is a message at bottom of screen to notify player of certain events.
 * UI Manager has a reference to this object and it's called from there.
 */

public class MessageNotification : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private bool IsActive;
    [Header("Text Component")]
    [SerializeField] private TextMeshProUGUI tmProNotificationMessage; 
    [Header("Positions")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform startingTransform;
    [Header("Movement Eases")]
    [SerializeField] LeanTweenType activateEase;
    [SerializeField] LeanTweenType deactivateEase;
    [Header("Speed of movement")]
    [SerializeField] private float moveTime;
    [Header("Time to stay on screen")]
    [SerializeField] private float timeToDissapear;

    private string currentNotificationMessage;
    private int hideTweenID;

    //Start.
    private void Awake() { }
    void Start()
    {
        transform.position = startingTransform.position;
        currentNotificationMessage = string.Empty;

        Key[] keysInGame = FindObjectsOfType<Key>();
        for (int i = 0; i < keysInGame.Length; i++)
        {
            keysInGame[i].PickedUpKeyEvent += MessageNotification_PickedUpKeyEvent;
        }
    }

    private void MessageNotification_PickedUpKeyEvent(KeyInventoryItem key)
    {
        Show(key.UIMessageToShowWhenPickedUp);
    }

    public void Show(string message)
    {
        if (IsActive && message != currentNotificationMessage) //Player activated another notification message while notification message was on screen.
        {            
            ResetMe();
            MoveMessageOnScreen(message);
        }
        else if (IsActive == false && LeanTween.isTweening(this.gameObject) == false)
            MoveMessageOnScreen(message);
    }
    public void Hide() => LeanTween.move(this.gameObject, startingTransform, moveTime).setEase(deactivateEase).setOnComplete(delegate () { IsActive = false; });

    private void MoveMessageOnScreen(string message)
    {
        IsActive = true;
        tmProNotificationMessage.text = message;
        currentNotificationMessage = message;

        LeanTween.move(this.gameObject, targetTransform, moveTime).setEase(activateEase);
        hideTweenID = LeanTween.delayedCall(moveTime + timeToDissapear, Hide).id;
    } 
    private void ResetMe()
    {
        LeanTween.cancel(this.gameObject);
        LeanTween.cancel(hideTweenID);

        this.gameObject.transform.position = startingTransform.transform.position;
    }
}
