using UnityEngine;

public class GhostEnemyNew : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stunTime = 2.5f;
    private Transform target;
    private SpriteRenderer spriteRenderer;

    // A variable to remember which spawner created this ghost.
    private GhostEnemySpawnerNew ownerSpawner;

    /// <summary>
    /// Called by the spawner right after this ghost is created.
    /// </summary>
    public void Initialize(GhostEnemySpawnerNew spawner)
    {
        this.ownerSpawner = spawner;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) target = playerObj.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
        spriteRenderer.flipX = dir.x < 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetStun(stunTime);
                Debug.Log("GhostEnemy hit Player");
                Destroy(gameObject);
            }
        }
    }
    
    private void Die()
    {
        // If this ghost knows who its spawner is, report its death.
        if (ownerSpawner != null)
        {
            ownerSpawner.OnGhostDied();
        }

        // Destroy this ghost instance.
        Destroy(gameObject);
    }
}