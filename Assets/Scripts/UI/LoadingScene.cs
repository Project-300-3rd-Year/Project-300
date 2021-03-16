using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


//Set up to just load the main scene, nothing else. Works for now.
//Text saying "loading scene" flashes during.
public class LoadingScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtLoading;
    [SerializeField] private float flashSpeed;
    private float flashTimer;

    private AsyncOperation loadSceneAsyncOperation;

    private void Start()
    {
        loadSceneAsyncOperation = SceneManager.LoadSceneAsync(2);
        loadSceneAsyncOperation.allowSceneActivation = true;
    }

    private void Update()
    {
        flashTimer += Time.deltaTime * flashSpeed;
        txtLoading.color = new Color(txtLoading.color.r, txtLoading.color.g, txtLoading.color.b, Mathf.PingPong(flashTimer, 1f));
    }
}
