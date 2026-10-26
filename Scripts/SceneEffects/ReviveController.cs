using UnityEngine;
using UnityEngine.SceneManagement;

public class ReviveController : MonoBehaviour
{
    public string playSceneName = "GameScene";

    public AudioSource reviveSfx;

    private Vector3 offset;
    private Vector3 startPosition;
    private bool isInReviveZone = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        newPosition.z = 0;
        transform.position = newPosition;
    }

    void OnMouseUp()
    {
        if (isInReviveZone)
        {
            if (reviveSfx != null) reviveSfx.Play();

            Invoke("LoadPlayScene", 1.0f);
        }
        else
        {
            transform.position = startPosition;
        }
    }

    void LoadPlayScene()
    {
        SceneManager.LoadScene(playSceneName);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ReviveZone")) isInReviveZone = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ReviveZone")) isInReviveZone = false;
    }
}