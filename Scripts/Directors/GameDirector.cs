using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    [Header("UI 연결")]
    public Image hpGaugeImage;
    public Image expGaugeImage;
    public GameObject helpPanel;
    public GameObject clearButton;

    [Header("시스템 연결")]
    public GameObject batGenerator;
    public PlayerController playerController;

    [Header("스탯 및 경험치 설정")]
    public int currentExp = 0;
    public int maxExp = 600;

    [Header("효과음 설정")]
    public AudioSource buttonSfx;

    [Header("BGM 설정")]
    public AudioSource bgmSource;
    public AudioClip clearBgmClip;

    void Start()
    {
        if (expGaugeImage != null)
        {
            expGaugeImage.fillAmount = (float)currentExp / maxExp;
        }

        if (clearButton != null)
        {
            clearButton.SetActive(false);
        }
    }

    public void UpdateHpUI(int currentHp, int maxHp)
    {
        if (hpGaugeImage != null)
        {
            hpGaugeImage.fillAmount = (float)currentHp / maxHp;
        }
    }

    public void ToggleHelpPanel()
    {
        if (helpPanel != null)
        {
            buttonSfx.Play();
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }

    public void AddExp(int expAmount)
    {
        currentExp += expAmount;

        if (currentExp >= maxExp)
        {
            currentExp = maxExp;

            if (batGenerator != null)
            {
                batGenerator.SetActive(false);
            }

            if (clearButton != null)
            {
                clearButton.SetActive(true);
            }

            if (playerController != null)
            {
                playerController.Victory();
            }

            if (bgmSource != null && clearBgmClip != null)
            {
                bgmSource.Stop();
                bgmSource.clip = clearBgmClip;
                bgmSource.Play();
            }
        }

        if (expGaugeImage != null)
        {
            expGaugeImage.fillAmount = (float)currentExp / maxExp;
        }
    }

    public void GoToClearScene()
    {
        if (buttonSfx != null) buttonSfx.Play();
        Invoke("LoadClearScene", 0.3f);
    }

    private void LoadClearScene()
    {
        SceneManager.LoadScene("GameClear");
    }
}