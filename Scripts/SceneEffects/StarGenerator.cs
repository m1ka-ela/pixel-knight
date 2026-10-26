using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [Header("생성 설정")]
    public GameObject starPrefab;
    public float spawnInterval = 0.1f;

    [Header("효과음 설정")]
    public AudioSource starSfx;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnStar();
        }
    }

    void SpawnStar()
    {
        if (starPrefab != null)
        {
            Instantiate(starPrefab, transform.position, Quaternion.identity);

            if (starSfx != null && starSfx.clip != null)
            {
                starSfx.PlayOneShot(starSfx.clip);
            }
        }
    }
}