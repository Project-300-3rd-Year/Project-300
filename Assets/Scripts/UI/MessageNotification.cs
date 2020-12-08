using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageNotification : MonoBehaviour
{
    [SerializeField] private Image _imgBackground; //Black background. 
    [SerializeField] private TextMeshProUGUI tmProNotificationMessage; //Message that appears.
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform startingTransform;
    [SerializeField] LeanTweenType activateEase;
    [SerializeField] LeanTweenType deactivateEase;

    public static MessageNotification Instance;
    [SerializeField] private float moveTime;
    float dissapearTimer;
    [SerializeField] private float timeToDissapear;

    [SerializeField] private bool IsActive;

    //Start.
    private void Awake() => Instance = this;
    void Start() => transform.position = startingTransform.position;

    void Update()
    {
        if(IsActive)
        {
            dissapearTimer += Time.deltaTime;
            if (dissapearTimer >= timeToDissapear)
            {
                IsActive = false;
                DeactivateNotificationMessage();
            }
        }
    }

    public void ActivateNotificationMessage(string message)
    {
        //_imgBackground.gameObject.SetActive(true);
        //tmProNotificationMessage.gameObject.SetActive(true);
        if(IsActive == false && LeanTween.isTweening(this.gameObject) == false)
        {
            IsActive = true;
            tmProNotificationMessage.text = message;

            LeanTween.move(this.gameObject, targetTransform, moveTime).setEase(activateEase);
        }
    }

    public void DeactivateNotificationMessage()
    {
        LeanTween.move(this.gameObject, startingTransform, moveTime).setEase(deactivateEase).setOnComplete(delegate() { dissapearTimer = 0; });
    }
}
