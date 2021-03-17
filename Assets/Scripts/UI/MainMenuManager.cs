﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image imgBlackBackground;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup settingsCanvasGroup;
    [SerializeField] private CanvasGroup howToPlayCanvasGroup;
    [SerializeField] private CanvasGroup currentCanvasGroup;

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
        GameManager.Instance.currentGameSessionState = GameSessonState.InMainMenu;

        cameraSensitivitySlider.value = Settings.cameraSensitivity; 
        UpdateCameraSensitivityValue();

        masterVolumeSlider.maxValue = Settings.maxVolume;
        masterVolumeSlider.value = Settings.volume;
        UpdateMasterVolumeValue();

        OnCameraArrivingAtUIScreen(mainMenuCanvasGroup);
    }

    //Main Menu.
    public void OnSettingsButtonPressed()
    {
        OnCameraLeavingUIScreen();

        LeanTween.rotate(Camera.main.gameObject,camTargetSettings.transform.rotation.eulerAngles, timeToRotateToSettings).setEase(rotateToSettingsEase);
        LeanTween.move(Camera.main.gameObject, camTargetSettings, timeToMoveToSettings).setEase(moveToSettingsEase)
            .setOnComplete(delegate() 
            {
                OnCameraArrivingAtUIScreen(canvasGroupToFadeIn: settingsCanvasGroup);
            });
    }
    public void OnHowToPlayButtonPressed()
    {
        OnCameraLeavingUIScreen();

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
        Settings.UpdateAudioListener(masterVolumeSlider.value);
    }
    public void OnReturnToMainMenuPressed()
    {
        //Update Settings.
        Settings.UpdateVolumeSettings(masterVolumeSlider.value);
        Settings.UpdateCameraSensitivity(cameraSensitivitySlider.value);

        OnCameraLeavingUIScreen();

        LeanTween.rotate(Camera.main.gameObject, camTargetMainMenu.transform.rotation.eulerAngles, timeToRotateToMainMenu).setEase(rotateToMainMenuEase);
        LeanTween.move(Camera.main.gameObject, camTargetMainMenu, timeToMoveToMainMenu).setEase(moveToMainMenuEase)
            .setOnComplete(delegate ()
            {
                OnCameraArrivingAtUIScreen(canvasGroupToFadeIn: mainMenuCanvasGroup);
            });
    }


    //On Moving from, Arriving at UI Screens.
    private void OnCameraLeavingUIScreen()
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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Confined;
        });
    }

    //Camera fade to black exit transition.
    public void FadeCameraOutToPlayGame() => StartCameraFadeTransition(() => SceneManager.LoadScene(1));
    public void FadeCameraOutToQuitGame() => StartCameraFadeTransition(() => Application.Quit(0));

    //For this transition I didn't bother making variables for move / rotate time etc.
    //Should make some if you wanted to customise the it more.
    public void StartCameraFadeTransition(Action delegateCalledAtEndOfSequence)
    {
        OnCameraLeavingUIScreen();

        //Fade out audio listener, seems to work fine for now - had to pass in a callOnUpdate Action to make the volume property change.
        LeanTween.value(this.gameObject, (float test) => AudioListener.volume = test, AudioListener.volume, 0f, 3f);

        LeanTween.rotate(Camera.main.gameObject, cameraTargetPosition1.transform.eulerAngles, 0.5f);
        LeanTween.move(Camera.main.gameObject, cameraTargetPosition1, 0.6f).setOnComplete(delegate ()
        {
            LeanTween.rotate(Camera.main.gameObject, cameraTargetPosition2.transform.eulerAngles, 0.8f);
            LeanTween.move(Camera.main.gameObject, cameraTargetPosition2, 3f);
            LeanTween.color(imgBlackBackground.rectTransform, new Color(imgBlackBackground.color.a, imgBlackBackground.color.g, imgBlackBackground.color.b, 1f), 2f).setOnComplete(delegateCalledAtEndOfSequence);
        });
    }
}
