using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("효과음 설정")]
    public AudioSource buttonSfx;

    public void GameStart()
    {
        PlayButtonSound();
        Invoke("LoadGameScene", 0.3f);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GameExit()
    {
        PlayButtonSound();
        Invoke("QuitGame", 0.3f);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void PlayButtonSound()
    {
        if (buttonSfx != null)
        {
            buttonSfx.Play();
        }
    }
}