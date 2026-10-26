using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearDirector : MonoBehaviour
{
    [Header("효과음 설정")]
    public AudioSource buttonSfx;

    public void GoToMenu()
    {
        if (buttonSfx != null) buttonSfx.Play();
        Invoke("LoadTitleScene", 0.3f);
    }

    public void QuitGame()
    {
        if (buttonSfx != null) buttonSfx.Play();
        Invoke("ExecuteQuit", 0.3f);
    }

    private void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    private void ExecuteQuit()
    {
        Application.Quit();
    }
}