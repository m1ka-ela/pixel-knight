using UnityEngine;

public class BatController : MonoBehaviour
{
    [Header("박쥐 스탯")]
    public float moveSpeed = 1.5f;
    public int hp = 20;
    public int attackDamage = 5;

    [Header("거리 판정 (가상의 콜라이더 역할)")]
    public float detectionRadius = 4.0f;
    public float attackRadius = 1.0f;

    [Header("애니메이션")]
    public Sprite[] idleSprites;
    public Sprite[] flySprites;
    public Sprite[] attack1Sprites;
    public Sprite[] attack2Sprites;
    public Sprite[] hurtSprites;
    public Sprite[] dieSprites;

    public float animSpeed = 0.1f;

    [Header("효과음 설정")]
    public AudioClip dieSfx;
    [Range(0f, 1f)]
    public float dieSfxVolume = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Sprite[] currentAnimSprites;
    private float time = 0;
    private int idx = 0;

    private bool isHurt = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool hasDealtDamage = false;

    private GameObject player;
    private PlayerController playerController;
    private Vector2 spawnPosition;
    private Vector2 randomTarget;
    private float waitTime = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        spawnPosition = transform.position;
        SetNewRandomTarget();
        ChangeAnimation(idleSprites);
    }

    void Update()
    {
        if (isDead || isHurt || isAttacking)
        {
            UpdateAnimation();
            return;
        }

        // 플레이어의 현재 체력을 확인하여 죽었는지 판별
        bool isPlayerDead = (playerController != null && playerController.currentHp <= 0);

        // 플레이어가 죽었다면 무조건 배회 상태로 전환
        if (isPlayerDead)
        {
            WanderAround();
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRadius)
            {
                StartAttack();
            }
            else if (distanceToPlayer <= detectionRadius)
            {
                MoveTowards(player.transform.position);
                ChangeAnimation(flySprites);
            }
            else
            {
                WanderAround();
            }
        }

        UpdateAnimation();
    }

    void StartAttack()
    {
        isAttacking = true;
        hasDealtDamage = false;

        if (Random.value > 0.5f)
        {
            ChangeAnimation(attack1Sprites);
        }
        else
        {
            ChangeAnimation(attack2Sprites);
        }
    }

    void MoveTowards(Vector2 targetPos)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (targetPos.x < transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }

    void WanderAround()
    {
        if (Vector2.Distance(transform.position, randomTarget) < 0.1f)
        {
            ChangeAnimation(idleSprites);
            waitTime -= Time.deltaTime;

            if (waitTime <= 0f)
            {
                SetNewRandomTarget();
                waitTime = Random.Range(1f, 3f);
            }
        }
        else
        {
            MoveTowards(randomTarget);
            ChangeAnimation(flySprites);
        }
    }

    void SetNewRandomTarget()
    {
        float randomX = spawnPosition.x + Random.Range(-2.0f, 2.0f);
        float randomY = spawnPosition.y + Random.Range(-2.0f, 2.0f);
        randomTarget = new Vector2(randomX, randomY);
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

                if (isDead)
                {
                    if (idx >= currentAnimSprites.Length)
                    {
                        idx = currentAnimSprites.Length - 1;
                    }
                }
                else if (isHurt)
                {
                    if (idx >= currentAnimSprites.Length)
                    {
                        isHurt = false;
                        ChangeAnimation(idleSprites);
                        return;
                    }
                }
                else if (isAttacking)
                {
                    if (idx == currentAnimSprites.Length / 2 && !hasDealtDamage)
                    {
                        // 공격 순간에도 플레이어가 살아있는지 확인
                        if (playerController != null && playerController.currentHp > 0)
                        {
                            if (Vector2.Distance(transform.position, player.transform.position) <= attackRadius)
                            {
                                playerController.TakeDamage(attackDamage);
                            }
                        }
                        hasDealtDamage = true;
                    }

                    if (idx >= currentAnimSprites.Length)
                    {
                        isAttacking = false;
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

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;
        isAttacking = false;

        if (hp <= 0)
        {
            hp = 0;
            isDead = true;

            GameObject director = GameObject.Find("GameDirector");
            if (director != null)
            {
                director.GetComponent<GameDirector>().AddExp(50);

                if (dieSfx != null)
                {
                    director.GetComponent<AudioSource>().PlayOneShot(dieSfx, dieSfxVolume);
                }
            }

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 2.0f;
            }

            Collider2D coll = GetComponent<Collider2D>();
            if (coll != null)
            {
                coll.isTrigger = false;
            }

            gameObject.layer = LayerMask.NameToLayer("Default");

            ChangeAnimation(dieSprites);
            Destroy(gameObject, 2.0f);
        }
        else
        {
            isHurt = true;
            ChangeAnimation(hurtSprites);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
        {
            Destroy(gameObject);
        }
    }
}