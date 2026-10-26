using UnityEngine;

public class HeartController : MonoBehaviour
{
    [Header("애니메이션")]
    public Sprite[] rotateSprites;
    public float animSpeed = 0.1f;

    [Header("Scaling 설정")]
    public float scaleSpeed = 3f;
    public float scaleAmount = 0.2f;

    [Header("회복 설정")]
    public int healAmount = 15;
    public AudioClip healSfx;

    private SpriteRenderer spriteRenderer;
    private Vector3 startScale;
    private float animTime = 0;
    private int idx = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;

        if (rotateSprites != null && rotateSprites.Length > 0)
        {
            spriteRenderer.sprite = rotateSprites[0];
        }

        if (startScale == Vector3.zero)
        {
            Debug.LogWarning(gameObject.name + "의 시작 스케일이 0입니다! 확인이 필요합니다.");
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

        float pulse = 1f + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
        transform.localScale = startScale * pulse;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Heal(healAmount);

                if (healSfx != null)
                {
                    other.GetComponent<AudioSource>().PlayOneShot(healSfx);
                }
                Destroy(gameObject);
            }
        }
    }
}