using UnityEngine;

public class StarController : MonoBehaviour
{
    [Header("애니메이션")]
    public Sprite[] rotateSprites;
    public float animSpeed = 0.1f;

    [Header("이동 궤적")]
    public float moveSpeed = 4.0f;
    public float archHeight = 4.0f;
    public float startX = -12f;
    public float endX = 12f;
    public float startY = 0f;

    private SpriteRenderer spriteRenderer;
    private float animTime = 0;
    private int idx = 0;
    private float currentX;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentX = startX;

        if (rotateSprites != null && rotateSprites.Length > 0)
        {
            spriteRenderer.sprite = rotateSprites[0];
        }
    }

    void Update()
    {
        if (rotateSprites != null && rotateSprites.Length > 0)
        {
            animTime += Time.deltaTime;
            if (animTime > animSpeed)
            {
                animTime = 0;
                idx = (idx + 1) % rotateSprites.Length;
                spriteRenderer.sprite = rotateSprites[idx];
            }
        }

        currentX += moveSpeed * Time.deltaTime;

        float totalDistance = endX - startX;
        float progress = (currentX - startX) / totalDistance;

        if (progress >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        float currentY = startY + (archHeight * Mathf.Sin(Mathf.PI * progress));

        transform.position = new Vector2(currentX, currentY);
    }
}