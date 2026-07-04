using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("움직임 설정")]
    public float bounceHeight = 0.5f; // 위로 튕길 높이
    public float bounceSpeed = 4f;    // 튕기는 속도
    
    private Vector2 originalPosition;  // 원래 벽돌 위치
    private bool isBouncing = false;   // 현재 튕기는 중인지 체크

    void Start()
    {
        originalPosition = transform.position;
    }

    public void Hit()
    {
        if (isBouncing) return;

        StartCoroutine(BounceSequence());
    }

    IEnumerator BounceSequence()
    {
        isBouncing = true;

        //1. 위로 이동
        Vector2 targetPosition = originalPosition + Vector2.up * bounceHeight;
        while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, bounceSpeed * Time.deltaTime);
            yield return null;
        }

        //2. 밑으로 내려옴
        while (Vector2.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPosition, bounceSpeed * Time.deltaTime);
            yield return null;
        }

        // 정확한 제자리 고정 및 상태 초기화
        transform.position = originalPosition;
        isBouncing = false;
    }
}