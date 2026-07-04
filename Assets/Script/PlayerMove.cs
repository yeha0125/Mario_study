using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    private bool isDead = false;
    private bool isClear = false;

    private Vector2 startPosition;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
        startPosition = transform.position;

        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider != null)
        {
            originalColliderSize = capsuleCollider.size;
            originalColliderOffset = capsuleCollider.offset;
        }
    }

    public void Shrink()
    {
        isBig = false;

        if (anim != null)
        {
            anim.SetBool("isBig", false);
            anim.SetTrigger("doShrink");
        }

        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider != null)
        {
            capsuleCollider.size = originalColliderSize;
            capsuleCollider.offset = originalColliderOffset;
        }
        StartCoroutine(ShrinkPauseSequence());
    }

    private IEnumerator ShrinkPauseSequence()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
    }

    void Update()           
    {
        if (isDead || isClear) return;

        //Jump
        if (Input.GetButtonDown("Jump")&&!anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
        }

        //Direction Sprite
        float h = Input.GetAxisRaw("Horizontal");

        if (h > 0)
            spriteRenderer.flipX = true;
        else if (h < 0)
            spriteRenderer.flipX = false;

        //Animation
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.3 || anim.GetBool("isJumping"))
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        if (isDead || isClear) return;

        //Move by Control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right*h,ForceMode2D.Impulse);

        //Right Max Speed
        if (rigid.linearVelocity.x > maxSpeed)
            rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);

        //Left Max Speed
        else if (rigid.linearVelocity.x < maxSpeed*(-1))
            rigid.linearVelocity = new Vector2(maxSpeed * (-1), rigid.linearVelocity.y);
    }

    //Landing Platform
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("LuckyBlock"))
        {
            if (collision.contacts[0].normal.y > 0.5f)
            {
                anim.SetBool("isJumping", false);
            }
        }

        if (collision.gameObject.CompareTag("DeathZone"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Flag") && !isClear)
        {
            StartCoroutine(ClearSequence());
        }
    }

    public void TakeDamage()
    {    
        if (isBig)
            Shrink();
        else
            Die();
    }
    

    public void Die()
    {
        if (isDead) return; 
        isDead = true;

        if (anim != null) anim.SetTrigger("doDie"); 

        // 위로 튕김
        GetComponent<Collider2D>().enabled = false; 
        rigid.linearVelocity = Vector2.zero;        
        rigid.AddForce(Vector2.up * 10f, ForceMode2D.Impulse); 

        Invoke("ProcessDeath", 2f);
    }

    void ProcessDeath()
    {
        if (StageManager.instance != null)
        {
            StageManager.instance.ReduceLife();
        }
    }

    public void Respawn()
    {
        isDead = false;
        transform.position = startPosition;
        GetComponent<Collider2D>().enabled = true;
        rigid.linearVelocity = Vector2.zero;
        
        if (anim != null)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isWalking", false);
        }

        isBig = false;
        if (anim != null) anim.SetBool("isBig", false);

        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider != null)
        {
            capsuleCollider.size = originalColliderSize;
            capsuleCollider.offset = originalColliderOffset;
        }
    }

    private bool isBig = false;

    public void GrowUp()
    {
        if (isBig) return;
        isBig = true;

        if (anim != null)
        {
            anim.SetBool("isBig", true);
            anim.SetTrigger("doGrow"); 
        }

        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider != null)
        {
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, 0.35f);
            capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, 0);
        }
        
        StartCoroutine(GrowPauseSequence());
    }

    private IEnumerator GrowPauseSequence()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
    }


    [Header("UI")]
    public GameObject clearCanvas;
    
    IEnumerator ClearSequence()
    {
        isClear = true;

        rigid.linearVelocity = Vector2.zero;
        rigid.gravityScale = 0.15f;
        
        float targetX = transform.position.x;
        anim.SetBool("isJumping", false);

        yield return new WaitForSeconds(0.1f); 

        while (Mathf.Abs(rigid.linearVelocity.y) > 0.05f)
        {
            transform.position = new Vector2(targetX, transform.position.y);
            yield return null;
        }

        rigid.gravityScale = 1f; 
        rigid.linearVelocity = Vector2.zero;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        yield return new WaitForSeconds(0.2f);

        float walkDuration = 2.5f;
        float elapsed = 0f;
        while (elapsed < walkDuration)
        {
            transform.position += Vector3.right * 2.5f * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        GetComponent<SpriteRenderer>().enabled = false;

        if (clearCanvas != null)
        {
            clearCanvas.SetActive(true);
        }
    }
}