using UnityEngine;
using System.Collections;

public class BombEnemyController : MonoBehaviour
{
    public float fallSpeed = 30f;
    public float explosionRadius = 1.5f;

    private Vector3 targetPosition;
    private bool hasLanded = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetTargetPosition(Vector3 target)
    {
        targetPosition = target;
        hasLanded = false;
    }

    void Update()
    {
        if (!hasLanded)
        {
            // 부드럽게 낙하
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

            // 도착하면 착지 처리
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                Land();
            }
        }
    }

    private void Land()
    {
        hasLanded = true;
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
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
                BaseBox box = hit.GetComponent<BaseBox>();
                box.DestroyBox();
            }
        }

        Destroy(gameObject); // 폭탄 제거
    }
}
