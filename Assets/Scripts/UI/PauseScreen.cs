using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Pause screen that gets activated on pressing P or ESC - checked for in UI Manager script.
 * pauseScreenDelegate gets updated depending on what screen you're on - so pressing P or ESC is same as pressing return / or back buttons etc.
 * Pause screen gameobject must be active in order for delegate to get set at start. 
 */

public class PauseScreen : MonoBehaviour
{
    public Action pauseScreenDelegate; //Method that gets called on pressing P or ESC.

    [Header("Pause Screen Panels")]
    [SerializeField] private GameObject mainPausePanel;
    [SerializeField] private GameObject settingsPausePanel;

    [Header("Pause Panel Elements")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI txtVolumeDisplay;
    [SerializeField] private Slider cameraSensitivitySlider;
    [SerializeField] private TextMeshProUGUI txtCameraSensitivityDisplay;

    //Button clicks.
    public void OnQuitGameButtonClicked() => Application.Quit(0);
    public void OnReturnToPausePanelButtonClicked() => EnableMainPausePanel();
    public void OnReturnToMainMenuButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void Start()
    {
        pauseScreenDelegate = EnablePauseScreen;

        volumeSlider.value = Settings.volume;
        cameraSensitivitySlider.value = Settings.cameraSensitivity;

        UpdateVolumeTextDisplay();
        UpdateCameraSensitivityTextDisplay();

        gameObject.SetActive(false);
    }

    //Overall Pause Screen.
    public void EnablePauseScreen()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pauseScreenDelegate = DisablePauseScreen;
    }
    public void DisablePauseScreen()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        pauseScreenDelegate = EnablePauseScreen;
    }

    //Panels Within Pause Screen - Main Panel or Settings Panel.
    public void EnableSettingsPanel()
    {
        mainPausePanel.SetActive(false);
        settingsPausePanel.SetActive(true);

        pauseScreenDelegate = EnableMainPausePanel;
    }
    public void EnableMainPausePanel()
    {
        //Save Settings.
        Settings.UpdateVolumeSettings(volumeSlider.value);
        Settings.UpdateCameraSensitivity(cameraSensitivitySlider.value);

        settingsPausePanel.SetActive(false);
        mainPausePanel.SetActive(true);

        pauseScreenDelegate = DisablePauseScreen;
    }

    //Settings Panel.
    public void UpdateVolumeTextDisplay()
    {
        txtVolumeDisplay.text = volumeSlider.value.ToString();
    }

    public void UpdateCameraSensitivityTextDisplay()
    {
        txtCameraSensitivityDisplay.text = cameraSensitivitySlider.value.ToString("0.00");
    }

}
