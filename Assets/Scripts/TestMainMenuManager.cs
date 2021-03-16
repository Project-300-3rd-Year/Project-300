using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestMainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image imgBlackBackground;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup settingsCanvasGroup;
    [SerializeField] private CanvasGroup howToPlayCanvasGroup;
    private CanvasGroup currentCanvasGroup;

    [Header("Settings UI Elements")]
    [SerializeField] private Slider cameraSensitivitySlider;
    [SerializeField] private TextMeshProUGUI txtCameraSensitivitySliderValue;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TextMeshProUGUI txtMasterVolumeSliderValue;

    [Header("Transitions")]
    [Header("Transition To Settings")]
    [SerializeField] private Transform camTargetSettings;
    [SerializeField] private float timeToMoveToSettings;
    [SerializeField] private LeanTweenType moveToSettingsEase;
    [SerializeField] private float timeToRotateToSettings;
    [SerializeField] private LeanTweenType rotateToSettingsEase;

    [Header("Transition To Settings")]
    [SerializeField] private Transform camTargetHowToPlay;
    [SerializeField] private float timeToMoveToHowToPlay;
    [SerializeField] private LeanTweenType moveToHowToPlayEase;
    [SerializeField] private float timeToRotateToHowToPlay;
    [SerializeField] private LeanTweenType rotateToHowToPlayEase;

    [Header("Transition To Main Menu")]
    [SerializeField] private Transform camTargetMainMenu;
    [SerializeField] private float timeToMoveToMainMenu;
    [SerializeField] private LeanTweenType moveToMainMenuEase;
    [SerializeField] private float timeToRotateToMainMenu;
    [SerializeField] private LeanTweenType rotateToMainMenuEase;

    [Header("All Canvas Groups Fade Time")]
    [SerializeField] private float timeToFadeCanvasGroups;

    [Header("CameraFadeTransition")]
    [SerializeField] Transform cameraTargetPosition1;
    [SerializeField] Transform cameraTargetPosition2;


    //Start.
    private void Start()
    {
        cameraSensitivitySlider.value = 3f; //READ FROM SETTINGS.
        UpdateCameraSensitivityValue();

        masterVolumeSlider.value = 5f; //READ FROM SETTINGS.
        UpdateMasterVolumeValue();

        currentCanvasGroup = mainMenuCanvasGroup;
    }

    //Main Menu.
    public void OnPlayButtonPressed() { }
    public void OnSettingsButtonPressed()
    {
        OnCameraBeginMovingToUIScreen();

        LeanTween.rotate(Camera.main.gameObject,camTargetSettings.transform.rotation.eulerAngles, timeToRotateToSettings).setEase(rotateToSettingsEase);
        LeanTween.move(Camera.main.gameObject, camTargetSettings, timeToMoveToSettings).setEase(moveToSettingsEase)
            .setOnComplete(delegate() 
            {
                OnCameraArrivingAtUIScreen(canvasGroupToFadeIn: settingsCanvasGroup);
            });
    }
    public void OnHowToPlayButtonPressed()
    {
        OnCameraBeginMovingToUIScreen();

        LeanTween.rotate(Camera.main.gameObject, camTargetHowToPlay.transform.rotation.eulerAngles, timeToRotateToHowToPlay).setEase(rotateToHowToPlayEase);
        LeanTween.move(Camera.main.gameObject, camTargetHowToPlay, timeToMoveToHowToPlay).setEase(moveToHowToPlayEase)
            .setOnComplete(delegate ()
            {
                OnCameraArrivingAtUIScreen(canvasGroupToFadeIn: howToPlayCanvasGroup);
            });


    }
    public void OnQuitButtonPressed()
    {
        Application.Quit(0);
    }

    //Settings.
    public void UpdateCameraSensitivityValue()
    {
        txtCameraSensitivitySliderValue.text = cameraSensitivitySlider.value.ToString("0.00");
    }
    public void UpdateMasterVolumeValue()
    {
        txtMasterVolumeSliderValue.text = masterVolumeSlider.value.ToString();
    }
    public void OnReturnToMainMenuPressed()
    {
        OnCameraBeginMovingToUIScreen();

        LeanTween.rotate(Camera.main.gameObject, camTargetMainMenu.transform.rotation.eulerAngles, timeToRotateToMainMenu).setEase(rotateToMainMenuEase);
        LeanTween.move(Camera.main.gameObject, camTargetMainMenu, timeToMoveToMainMenu).setEase(moveToMainMenuEase)
            .setOnComplete(delegate ()
            {
                OnCameraArrivingAtUIScreen(canvasGroupToFadeIn: mainMenuCanvasGroup);
            });
    }


    //On Moving from, Arriving at UI Screens.
    private void OnCameraBeginMovingToUIScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LeanTween.alphaCanvas(currentCanvasGroup, 0, timeToFadeCanvasGroups).setOnComplete(() => currentCanvasGroup.gameObject.SetActive(false));
    }
    private void OnCameraArrivingAtUIScreen(CanvasGroup canvasGroupToFadeIn)
    {
        currentCanvasGroup = canvasGroupToFadeIn;
        canvasGroupToFadeIn.gameObject.SetActive(true);

        LeanTween.alphaCanvas(canvasGroupToFadeIn, 1f, timeToFadeCanvasGroups).setOnComplete(delegate() 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        });
    }

    //Transition on play and quit.
    public void StartCameraFadeTransition()
    {
        OnCameraBeginMovingToUIScreen();

        LeanTween.rotate(Camera.main.gameObject, cameraTargetPosition1.transform.eulerAngles, 0.5f);
        LeanTween.move(Camera.main.gameObject, cameraTargetPosition1, 0.6f).setOnComplete(delegate ()
        {
            LeanTween.rotate(Camera.main.gameObject, cameraTargetPosition2.transform.eulerAngles, 0.8f);
            LeanTween.move(Camera.main.gameObject, cameraTargetPosition2, 3f);
            LeanTween.color(imgBlackBackground.rectTransform, new Color(imgBlackBackground.color.a, imgBlackBackground.color.g, imgBlackBackground.color.b, 1f), 2f);
        });
    }
}
