using UnityEngine;

public class GoombaMove : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] float moveSpeed = 150f;
    [SerializeField] float flipTime = 2f;

    private bool isRight = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D monsterRb;
    private Animator monsterAnim;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        monsterRb = GetComponent<Rigidbody2D>();
        monsterAnim = GetComponent<Animator>();
        
        // flipTime 주기로 방향 Flip 함수를 반복 실행
        InvokeRepeating("Flip", flipTime, flipTime);
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float targetVelocityX = isRight ? moveSpeed * Time.fixedDeltaTime : -moveSpeed * Time.fixedDeltaTime;
        monsterRb.linearVelocityX = targetVelocityX;
    }

    void Flip()
    {
        isRight = !isRight;
        spriteRenderer.flipX = isRight; // 방향에 맞춰 이미지 반전
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMove player = other.gameObject.GetComponent<PlayerMove>();
            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
            Animator playerAnim = other.gameObject.GetComponent<Animator>();

            if (player == null || playerRb == null) return;

            if (other.contacts[0].normal.y < -0.5f)
            {
                if (playerAnim != null) playerAnim.SetBool("isJumping", true);
                
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, player.jumpPower);

                Destroy(gameObject);
            }
            else
            {
                player.TakeDamage();
            }
        }
    }
}