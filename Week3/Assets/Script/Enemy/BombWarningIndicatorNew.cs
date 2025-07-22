using UnityEngine;
using System.Collections;

public class BombWarningIndicatorNew : MonoBehaviour
{
    private Transform player;
    private BombRainSpawner ownerSpawner; // 나를 생성한 스포너
    private SpriteRenderer spriteRenderer;
    private float moveSpeed = 4f;

    // 설정값
    public float followDuration = 3f;
    public float fadeDuration = 1f;
    public float maxScale = 16f;
    public float startAlpha = 0.4f;
    public float endAlpha = 0.8f;

    // Init 함수가 스포너를 인자로 받도록 수정
    public void Init(Transform targetPlayer, BombRainSpawner spawner)
    {
        player = targetPlayer;
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
                Vector3 targetPos = new Vector3(player.position.x, player.position.y, 0f);
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

        // 2. 위치 고정 + 알파 점점 진해짐
        timer = 0f;
        Vector3 fixedPosition = transform.position; // 현재 위치 고정
        while (timer < fadeDuration)
        {
            transform.position = fixedPosition;

            float t = timer / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            SetAlpha(alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        //yield return new WaitForSeconds(followDuration + fadeDuration); // 간단하게 합침
        
        // 역할이 끝나면, 기억해 둔 스포너에게 폭탄 투하를 요청
        if (ownerSpawner != null)
        {
            ownerSpawner.DropBombAt(transform.position);
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