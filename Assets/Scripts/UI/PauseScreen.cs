using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    //Enabling / Disabling.
    public void Enable()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ActivateOrDeactivate()
    {
        if (gameObject.activeSelf == true)
            Disable();
        else
            Enable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
}
