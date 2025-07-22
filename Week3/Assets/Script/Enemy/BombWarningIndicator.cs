using UnityEngine;
using System.Collections;

public class BombWarningIndicator : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer spriteRenderer;

    private float moveSpeed = 4f;

    public void Init(Transform targetPlayer)
    {
        player = targetPlayer;

        // 플레이어 주변 랜덤 위치에서 시작
        Vector2 offset = Random.insideUnitCircle.normalized * 3f;
        transform.position = player.position + (Vector3)offset;

        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(WarningRoutine());
    }

    private IEnumerator WarningRoutine()
    {
        float followDuration = 3f;
        float blinkDuration = 1f;
        float timer = 0f;

        // 0~3초: 따라가며 점점 진해짐
        while (timer < followDuration)
        {
            if (player != null)
            {
                Vector3 targetPos = new Vector3(player.position.x, player.position.y, 0f);
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
            }

            // 점점 진해짐
            float alpha = Mathf.Clamp01(timer / followDuration);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            timer += Time.deltaTime;
            yield return null;
        }

        // 3~4초: 위치 고정 + 깜빡임
        float blinkTimer = 0f;
        while (blinkTimer < blinkDuration)
        {
            float alpha = Mathf.PingPong(blinkTimer * 8f, 1f); // 빠르게 깜빡임
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            blinkTimer += Time.deltaTime;
            yield return null;
        }

        // 폭탄 낙하
        FindObjectOfType<BombRainController>()?.DropBombAt(transform.position);
        Destroy(gameObject);
    }
}
