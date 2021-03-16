using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestMainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup settingsPanel;

    [Header("Settings UI Elements")]
    [SerializeField] private Slider cameraSensitivitySlider;
    [SerializeField] private TextMeshProUGUI txtCameraSensitivitySliderValue;

    [Header("Transitions")]
    [Header("Transition To Settings")]
    [SerializeField] private Transform camTargetSettings;
    [SerializeField] private float timeToMoveToSettings;
    [SerializeField] private LeanTweenType moveToSettingsEase;
    [SerializeField] private float timeToRotateToSettings;
    [SerializeField] private LeanTweenType rotateToSettingsEase;

    [Header("Transition To Main Menu")]
    [SerializeField] private Transform camTargetMainMenu;
    [SerializeField] private float timeToMoveToMainMenu;
    [SerializeField] private LeanTweenType moveToMainMenuEase;
    [SerializeField] private float timeToRotateToMainMenu;
    [SerializeField] private LeanTweenType rotateToMainMenuEase;


    [SerializeField] private float timeToFadeCanvasGroups;


    //Start.
    private void Start()
    {
        cameraSensitivitySlider.value = 3f; //READ FROM SETTINGS.
        UpdateCameraSensitivityValue();
    }

    //Button presses.
    //From Main Menu.
    public void OnPlayButtonPressed() { }
    public void OnSettingsButtonPressed()
    {
        OnCameraBeginMovingToUIScreen(canvasGroupToFadeOut: mainMenuPanel);

        LeanTween.rotate(Camera.main.gameObject,camTargetSettings.transform.rotation.eulerAngles, timeToRotateToSettings).setEase(rotateToSettingsEase);
        LeanTween.move(Camera.main.gameObject, camTargetSettings, timeToMoveToSettings).setEase(moveToSettingsEase)
            .setOnComplete(delegate() 
            {
                OnCameraArrivingAtUIScreen(canvasGroupToFadeIn: settingsPanel);
            });
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit(0);
    }

    //From Settings.
    public void OnReturnToMainMenuPressed()
    {
        OnCameraBeginMovingToUIScreen(canvasGroupToFadeOut: settingsPanel);

        LeanTween.rotate(Camera.main.gameObject, camTargetMainMenu.transform.rotation.eulerAngles, timeToRotateToMainMenu).setEase(rotateToMainMenuEase);
        LeanTween.move(Camera.main.gameObject, camTargetMainMenu, timeToMoveToMainMenu).setEase(moveToMainMenuEase)
            .setOnComplete(delegate ()
            {
                OnCameraArrivingAtUIScreen(canvasGroupToFadeIn: mainMenuPanel);
            });
    }


    public void UpdateCameraSensitivityValue()
    {
        txtCameraSensitivitySliderValue.text = cameraSensitivitySlider.value.ToString("0.00");
    }


    //On Moving from, Arriving at UI Screens.
    private void OnCameraBeginMovingToUIScreen(CanvasGroup canvasGroupToFadeOut)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LeanTween.alphaCanvas(canvasGroupToFadeOut, 0, timeToFadeCanvasGroups).setOnComplete(() => canvasGroupToFadeOut.gameObject.SetActive(false));
    }
    private void OnCameraArrivingAtUIScreen(CanvasGroup canvasGroupToFadeIn)
    {
        canvasGroupToFadeIn.gameObject.SetActive(true);

        LeanTween.alphaCanvas(canvasGroupToFadeIn, 1f, timeToFadeCanvasGroups).setOnComplete(delegate() 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        });
    }
}
