using UnityEngine;
using System.Collections;

public class BombWarningIndicator : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer spriteRenderer;

    private float moveSpeed = 4f;

    // 설정값
    public float followDuration = 3f;
    public float fadeDuration = 1f;
    public float maxScale = 16f;
    public float startAlpha = 0.4f;
    public float endAlpha = 0.8f;

    public void Init(Transform targetPlayer)
    {
        player = targetPlayer;

        // 플레이어 주변 랜덤 위치에서 시작
        Vector2 offset = Random.insideUnitCircle.normalized * 3f;
        transform.position = player.position + (Vector3)offset;

        spriteRenderer = GetComponent<SpriteRenderer>();

        // 초기 설정
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
            transform.localScale = new Vector3(currentScale, currentScale, transform.localScale.z);

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

        // 3. 슬라임 낙하
        //FindObjectOfType<BombRainController>()?.DropBombAt(transform.position);
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
