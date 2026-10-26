using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverDirector : MonoBehaviour
{
    [Header("효과음 설정")]
    public AudioSource buttonSfx;

    public void GoToMenu()
    {
        if (buttonSfx != null) buttonSfx.Play();
        Invoke("LoadTitleScene", 0.3f);
    }

    private void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}