using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    public float explosionRadius = 2f;
    public float delay = 0.5f;

    void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController pc = hit.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.HitByBomb();
                }
            }
            else if (hit.CompareTag("Box"))
            {
                Destroy(hit.gameObject); // 상자 파괴
            }
        }

        Destroy(gameObject); // 폭탄 제거
    }
}
