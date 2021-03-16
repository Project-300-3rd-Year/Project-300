using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMainMenu : MonoBehaviour
{
    AsyncOperation loadSceneProgress;
    Scene mainMenuScene;
    

    public TextMeshProUGUI txtProgress;

    private void Awake()
    {
        mainMenuScene = SceneManager.GetActiveScene();
    }

    public void OnLoadSceneButtonClicked()
    {
        StartCoroutine(LoadScene());
    }

    private void Update()
    {
        if (loadSceneProgress != null)
            print(loadSceneProgress.isDone);
    }

    private IEnumerator LoadScene()
    {
        loadSceneProgress = SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
        loadSceneProgress.allowSceneActivation = true;
        yield return null;
    }

}
