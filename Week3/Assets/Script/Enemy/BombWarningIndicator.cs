using UnityEngine;
using System.Collections;

public class BombWarningIndicator : MonoBehaviour
{
    private Transform player;
    private bool followPlayer = true;

    public void Init(Transform targetPlayer)
    {
        player = targetPlayer;
        followPlayer = true;
        StartCoroutine(WarningRoutine());
    }

    private IEnumerator WarningRoutine()
    {
        float followTime = 3f;
        float timer = 0f;

        while (timer < followTime)
        {
            if (player != null && followPlayer)
            {
                transform.position = new Vector3(player.position.x, player.position.y, 0f);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        followPlayer = false; // 고정

        yield return new WaitForSeconds(1f); // 1초 대기 후 폭탄 떨어뜨림

        FindObjectOfType<BombRainController>()?.DropBombAt(transform.position);
        Destroy(gameObject);
    }
}

