using UnityEngine;

public class BatGenerator : MonoBehaviour
{
    public GameObject batPrefab;
    public float spawnInterval = 3.0f;
    public float padding = 1.0f;

    [Header("스폰 Y좌표 제한")]
    public float minYLimit = -2.0f;

    private float delta = 0f;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        delta += Time.deltaTime;

        if (delta > spawnInterval)
        {
            delta = 0f;

            Vector2 camPos = mainCam.transform.position;
            float halfHeight = mainCam.orthographicSize;
            float halfWidth = halfHeight * mainCam.aspect;

            float minX = camPos.x - halfWidth - padding;
            float maxX = camPos.x + halfWidth + padding;

            float minY = Mathf.Max(camPos.y - halfHeight - padding, minYLimit);
            float maxY = camPos.y + halfHeight + padding;

            if (maxY < minYLimit) return;

            float px = Random.Range(minX, maxX);
            float py = Random.Range(minY, maxY);

            GameObject bat = Instantiate(batPrefab) as GameObject;
            bat.transform.position = new Vector3(px, py, 0);
        }
    }
}