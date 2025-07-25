using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform target; // Player
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null)
        {
            Debug.LogError("Player not found");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;

            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;

        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;

        if (dir.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetStun(2.5f);
                Debug.Log("GhostEnemy hit Player");
                Destroy(gameObject);
            }
        }
    }
}
