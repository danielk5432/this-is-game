using UnityEngine;
using System.Collections;

public class BombWarningIndicatorNew : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private BombRainSpawner ownerSpawner; // 나를 생성한 스포너
    private SpriteRenderer spriteRenderer;
    private float moveSpeed = 1.8f;
    private float velocityFollowFactor = 1f; // 플레이어 속도에 따라 따라다니는 정도

    // 설정값
    public float followDuration = 3f;
    public float fadeDuration = 0.5f;
    public float fallTiming = 0.4f; // 경고 표시기가 따라다니다가 떨어지는 시간
    public float maxScale = 16f;
    public float startAlpha = 0.2f;
    public float endAlpha = 0.8f;

    // Init 함수가 스포너를 인자로 받도록 수정
    public void Init(Transform targetPlayer, BombRainSpawner spawner)
    {
        player = targetPlayer;
        rb = player.GetComponent<Rigidbody2D>();
        ownerSpawner = spawner; // 스포너를 기억

        transform.position = player.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one;
        SetAlpha(startAlpha);
        StartCoroutine(WarningRoutine());
    }

    private IEnumerator WarningRoutine()
    {
        float timer = 0f;

        // 1. 따라다니며 점점 커짐
        while (timer < followDuration)
        {
            if (player != null)
            {
                Vector3 targetPos = new Vector3(player.position.x + rb.linearVelocity.x * velocityFollowFactor, player.position.y + rb.linearVelocity.y * velocityFollowFactor, 0f); // 플레이어 위치 + 약간의 속도 보정
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
            }

            // Scale 점점 증가
            // Scale 점점 증가 (z는 그대로 유지)
            float scaleT = timer / followDuration;
            float currentScale = Mathf.Lerp(1f, maxScale, scaleT);
            transform.localScale = new Vector3(currentScale, currentScale * 0.6f, transform.localScale.z);

            // 알파값은 일정하게 유지
            SetAlpha(startAlpha);

            timer += Time.deltaTime;
            yield return null;
        }

        bool drop = false;


        // 2. 위치 고정 + 알파 점점 진해짐
        timer = 0f;
        Vector3 fixedPosition = transform.position; // 현재 위치 고정
        while (timer < fadeDuration)
        {
            if (ownerSpawner != null && !drop && timer >= fadeDuration - fallTiming)
            {
                drop = true; // 한 번만 드롭
                ownerSpawner.DropBombAt(transform.position);
            }

            transform.position = fixedPosition;

            float t = timer / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            SetAlpha(alpha);

            timer += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    private void SetAlpha(float a)
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = a;
            spriteRenderer.color = c;
        }
    }
}