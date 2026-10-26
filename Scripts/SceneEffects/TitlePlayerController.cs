using UnityEngine;

public class TitlePlayerController : MonoBehaviour
{
    [Header("애니메이션")]
    public Sprite[] walkSprites;
    public float animSpeed = 0.1f;

    private SpriteRenderer spriteRenderer;
    private float time = 0;
    private int idx = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (walkSprites != null && walkSprites.Length > 0)
        {
            spriteRenderer.sprite = walkSprites[0];
        }
    }

    void Update()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (walkSprites != null && walkSprites.Length > 0)
        {
            time += Time.deltaTime;

            if (time > animSpeed)
            {
                time = 0;
                idx++;

                idx = idx % walkSprites.Length;

                spriteRenderer.sprite = walkSprites[idx];
            }
        }
    }
}