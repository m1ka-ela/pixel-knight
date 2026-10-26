using UnityEngine;

public class GameOverPlayerController : MonoBehaviour
{
    [Header("애니메이션")]
    public Sprite[] dieSprites;
    public float animSpeed = 0.1f;

    private SpriteRenderer spriteRenderer;
    private float time = 0;
    private int idx = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (dieSprites != null && dieSprites.Length > 0)
        {
            spriteRenderer.sprite = dieSprites[0];
        }
    }

    void Update()
    {
        if (dieSprites == null || dieSprites.Length == 0) return;

        if (idx >= dieSprites.Length - 1) return;

        time += Time.deltaTime;

        if (time > animSpeed)
        {
            time = 0;
            idx++;
            spriteRenderer.sprite = dieSprites[idx];
        }
    }
}