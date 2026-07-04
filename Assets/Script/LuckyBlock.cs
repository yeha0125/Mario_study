using UnityEngine;
using System.Collections;

public class LuckyBlock : MonoBehaviour
{
    public Sprite emptyBlockSprite;
    public GameObject itemPrefab;
    
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 4f;

    private Vector2 originalPosition;
    private bool isAlreadyHit = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAlreadyHit || !collision.gameObject.CompareTag("Player")) return;

        if (collision.contacts[0].normal.y > 0.5f)
        {
            TriggerBlock();
        }
    }

    private void TriggerBlock()
    {
        isAlreadyHit = true;
        
        StartCoroutine(BounceSequence());

        if (itemPrefab != null)
        {
            StartCoroutine(SpawnMushroomSequence());
        }

        if (emptyBlockSprite != null)
        {
            spriteRenderer.sprite = emptyBlockSprite;
        }
        
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;
    }

    // 블록 움직이는거
    private IEnumerator BounceSequence()
    {
        Vector2 targetPosition = originalPosition + Vector2.up * bounceHeight;

        while (MoveTowards(targetPosition)) yield return null;
        while (MoveTowards(originalPosition)) yield return null;
    }

    private bool MoveTowards(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, bounceSpeed * Time.deltaTime);
        return (Vector2)transform.position != target;
    }

    // 버섯 솟아오름
    private IEnumerator SpawnMushroomSequence()
    {
        // 버섯 생성
        GameObject mushroom = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        
        Rigidbody2D mushroomRb = mushroom.GetComponent<Rigidbody2D>();
        Collider2D mushroomCollider = mushroom.GetComponent<Collider2D>();
        MushroomItem mushroomScript = mushroom.GetComponent<MushroomItem>();

        if (mushroomRb != null) mushroomRb.isKinematic = true;
        if (mushroomCollider != null) mushroomCollider.enabled = false;
        if (mushroomScript != null) mushroomScript.enabled = false;

        Vector3 targetSpawnPos = transform.position + Vector3.up * 1f;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            mushroom.transform.position = Vector3.Lerp(transform.position, targetSpawnPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mushroom.transform.position = targetSpawnPos;

        // 버섯 걸음
        if (mushroomRb != null) mushroomRb.isKinematic = false;
        if (mushroomCollider != null) mushroomCollider.enabled = true;
        if (mushroomScript != null) mushroomScript.enabled = true;
    }
}
