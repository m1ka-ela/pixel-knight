using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("기본 스탯")]
    public int maxHp = 50;
    public int currentHp;
    public float moveSpeed = 5.0f;
    public float jumpForce = 500.0f;

    [Header("공격 설정")]
    public Transform attackPoint;
    public float attackRange = 1.0f;
    public int attackDamage = 10;
    public LayerMask enemyLayers;

    [Header("애니메이션")]
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    public Sprite[] jumpSprites;
    public Sprite[] fallSprites;
    public Sprite[] attack1Sprites;
    public Sprite[] attack2Sprites;
    public Sprite[] hurtSprites;
    public Sprite[] dieSprites;
    public Sprite[] victorySprites;

    public float animSpeed = 0.1f;

    [Header("효과음 설정")]
    public AudioClip walkSfx;
    public AudioClip jumpSfx;
    public AudioClip landSfx;
    public AudioClip attackSfx;
    public AudioClip hurtSfx;
    public AudioClip dieSfx;
    public AudioClip fallDieSfx;

    private AudioSource audioSource;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Sprite[] currentAnimSprites;
    private float time = 0;
    private int idx = 0;

    private bool isGrounded = false;
    private bool isHurt = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool hasDealtDamage = false;
    private bool isVictorious = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        currentHp = maxHp;
        ChangeAnimation(idleSprites);
    }

    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        if (!wasGrounded && isGrounded && !isDead)
        {
            PlaySound(landSfx);
        }

        if (isDead)
        {
            if (isGrounded && currentAnimSprites != dieSprites)
            {
                ChangeAnimation(dieSprites);
                rb.linearVelocity = Vector2.zero;
            }
            UpdateAnimation();
            return;
        }

        // 승리 상태일 경우 다른 입력을 무시하고 애니메이션만 재생
        if (isVictorious)
        {
            UpdateAnimation();
            return;
        }

        if (isHurt)
        {
            UpdateAnimation();
            return;
        }

        HandleInput();
        UpdateAnimation();
    }

    void HandleInput()
    {
        if (isAttacking) return;

        // 이동 (A, D)
        float h = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);

        if (h != 0)
        {
            spriteRenderer.flipX = (h < 0);
            if (isGrounded)
            {
                ChangeAnimation(walkSprites);

                if (!audioSource.isPlaying)
                {
                    PlaySound(walkSfx);
                }
            }
        }
        else
        {
            if (isGrounded) ChangeAnimation(idleSprites);
        }

        // 점프 (Space)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce));
            ChangeAnimation(jumpSprites);
            PlaySound(jumpSfx);
        }

        // 공중 모션 처리
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                ChangeAnimation(jumpSprites);
            }
            else if (rb.linearVelocity.y < -0.1f)
            {
                ChangeAnimation(fallSprites);
            }
        }

        // 공격 (마우스 클릭)
        if (!isAttacking)
        {
            // 현재 마우스 포인터가 캔버스 UI 요소 위에 있는지 확인
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return; // UI를 클릭한 상태라면 아래 공격 코드를 실행하지 않고 빠져나감
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartAttack(attack1Sprites);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                StartAttack(attack2Sprites);
            }
        }
    }

    void StartAttack(Sprite[] attackAnim)
    {
        isAttacking = true;
        hasDealtDamage = false;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        ChangeAnimation(attackAnim);
        PlaySound(attackSfx);
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

                // 1. 사망, 점프(Fall) 모션 고정 처리
                if (isDead || (!isGrounded && currentAnimSprites == jumpSprites) || currentAnimSprites == fallSprites)
                {
                    if (idx >= currentAnimSprites.Length)
                    {
                        idx = currentAnimSprites.Length - 1;
                    }
                }
                // 2. 피격 처리
                else if (isHurt)
                {
                    if (idx >= currentAnimSprites.Length)
                    {
                        isHurt = false;
                        if (isVictorious) ChangeAnimation(victorySprites);
                        else ChangeAnimation(idleSprites);
                        return;
                    }
                }
                // 3. 공격 처리
                else if (isAttacking)
                {
                    if (idx == currentAnimSprites.Length / 2 && !hasDealtDamage)
                    {
                        PerformAttack();
                    }

                    if (idx >= currentAnimSprites.Length)
                    {
                        isAttacking = false;
                        ChangeAnimation(idleSprites);
                        return;
                    }
                }
                // 4. 일반 루프 애니메이션 (Walk, Idle, Victory 모션 반복)
                else
                {
                    idx = idx % currentAnimSprites.Length;
                }

                spriteRenderer.sprite = currentAnimSprites[idx];
            }
        }
    }

    void PerformAttack()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            BatController bat = enemy.GetComponent<BatController>();
            if (bat != null)
            {
                bat.TakeDamage(attackDamage);
            }
        }

        hasDealtDamage = true;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        isAttacking = false;

        GameObject director = GameObject.Find("GameDirector");
        if (director != null)
        {
            director.GetComponent<GameDirector>().UpdateHpUI(currentHp, maxHp);
        }

        if (currentHp <= 0)
        {
            currentHp = 0;
            isDead = true;

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (isGrounded)
            {
                ChangeAnimation(dieSprites);
            }
            else
            {
                ChangeAnimation(fallSprites);
            }

            PlaySound(dieSfx);
            Invoke("LoadGameOverScene", 1.0f);
        }
        else
        {
            isHurt = true;
            ChangeAnimation(hurtSprites);
            PlaySound(hurtSfx);
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHp += amount;

        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }

        GameObject director = GameObject.Find("GameDirector");
        if (director != null)
        {
            director.GetComponent<GameDirector>().UpdateHpUI(currentHp, maxHp);
        }

        Debug.Log("체력 회복! 현재 HP: " + currentHp);
    }

    public void Victory()
    {
        if (isDead) return;

        isVictorious = true;
        isAttacking = false;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        ChangeAnimation(victorySprites);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FallZone"))
        {
            if (!isDead)
            {
                isDead = true;

                ChangeAnimation(fallSprites);

                PlaySound(fallDieSfx);

                Invoke("ReloadCurrentScene", 1.0f);
            }
        }
    }

    void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}