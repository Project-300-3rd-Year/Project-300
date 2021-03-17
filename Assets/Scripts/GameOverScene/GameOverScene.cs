using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameEndingTextCanvasGroup;
    [SerializeField] private CanvasGroup buttonPanelCanvasGroup;
    [SerializeField] private TextMeshProUGUI txtGameOverMessage;

    [Header("Sequence of appearing")]
    [SerializeField] private float timeToFadeInGameEndingText;
    [SerializeField] private float timeDelayBetweenShowingButtonPanel;
    [SerializeField] private float timeToFadeInButtonPanel;

    [SerializeField] private string winMessage;
    [SerializeField] private string loseMessage;
    private string gameEndMessage;

    private void Start()
    {
        switch (GameManager.Instance.currentGameSessionState)
        {
            case GameSessonState.GameOverWin:
                gameEndMessage = winMessage;
                break;
            case GameSessonState.GameOverLose:
                gameEndMessage = loseMessage;
                break;
            default:
                gameEndMessage = string.Empty;
                break;
        }

        txtGameOverMessage.text = gameEndMessage;


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Play sequence of appearing.
        LeanTween.alphaCanvas(gameEndingTextCanvasGroup, 1f, timeToFadeInGameEndingText).setOnComplete(() => 
        LeanTween.delayedCall(timeDelayBetweenShowingButtonPanel,delegate() 
        {
            LeanTween.alphaCanvas(buttonPanelCanvasGroup,1f, timeToFadeInButtonPanel).setOnComplete(delegate() 
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;                
            });       
        }));
    }

    //Button Clicks Events.
    public void OnPlayAgainButtonClicked() => SceneManager.LoadScene(1);
    public void OnMainMenuButtonClicked() => SceneManager.LoadScene(0);
    public void OnQuitButtonClicked() => Application.Quit(0);


}
