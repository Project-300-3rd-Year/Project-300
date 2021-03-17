using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Player collides with trigger - game ends.
public class EndGameTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.onGameEnd();
            UIManager.Instance.imgFadeToBlack.FadeToBlack(delegate() 
            {
                GameManager.Instance.currentGameSessionState = GameSessonState.GameOverWin;
                SceneManager.LoadScene(3);
            });
        }
    }
}
