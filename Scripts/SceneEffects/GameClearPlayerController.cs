using UnityEngine;

public class GameClearPlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5.0f;
    public float targetX = 0f;

    [Header("애니메이션")]
    public Sprite[] rollSprites;
    public Sprite[] victorySprites;
    public Sprite[] idleSprites;

    public float animSpeed = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Sprite[] currentAnimSprites;
    private float time = 0;
    private int idx = 0;

    private enum State { Rolling, Victory, Idle }
    private State currentState = State.Rolling;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        ChangeAnimation(rollSprites);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Rolling:
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, transform.position.y), moveSpeed * Time.deltaTime);

                if (Mathf.Abs(transform.position.x - targetX) < 0.01f)
                {
                    currentState = State.Victory;
                    ChangeAnimation(victorySprites);
                }
                break;

            case State.Victory:
                break;

            case State.Idle:
                break;
        }

        UpdateAnimation();
    }

    void ChangeAnimation(Sprite[] newAnimSprites)
    {
        if (currentAnimSprites == newAnimSprites) return;

        currentAnimSprites = newAnimSprites;
        idx = 0;
        time = 0f;

        if (currentAnimSprites != null && currentAnimSprites.Length > 0)
        {
            spriteRenderer.sprite = currentAnimSprites[0];
        }
    }

    void UpdateAnimation()
    {
        if (currentAnimSprites != null && currentAnimSprites.Length > 0)
        {
            time += Time.deltaTime;

            if (time > animSpeed)
            {
                time = 0;
                idx++;

                if (currentState == State.Victory)
                {
                    if (idx >= currentAnimSprites.Length)
                    {
                        currentState = State.Idle;
                        ChangeAnimation(idleSprites);
                        return;
                    }
                }
                else
                {
                    idx = idx % currentAnimSprites.Length;
                }

                spriteRenderer.sprite = currentAnimSprites[idx];
            }
        }
    }
}