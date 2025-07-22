using UnityEngine;
using Pathfinding; // A* Pathfinding Project를 사용하기 위해 필요

[RequireComponent(typeof(Seeker), typeof(AIPath))]
public class GhostEnemyNew : MonoBehaviour
{
    public float stunTime = 2.5f;
    private Transform target;
    private SpriteRenderer spriteRenderer;
    private GhostEnemySpawnerNew ownerSpawner;
    private AIPath aiPath;

    public void Initialize(GhostEnemySpawnerNew spawner)
    {
        ownerSpawner = spawner;
        target = PlayerSpawner.playerInstance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        aiPath = GetComponent<AIPath>();

        // AIPath will handle movement, so we need to tell it where to go.
        if (aiPath != null && target != null)
        {
            aiPath.destination = target.position;
        }
    }

    void Update()
    {
        if (target == null) return;

        // Constantly update the destination to follow the player
        aiPath.destination = target.position;

        // Flip sprite based on the AI's desired velocity
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (aiPath.desiredVelocity.x <= -0.01f)
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
                pc.SetStun(stunTime);
                Die();
            }
        }
    }
    
    private void Die()
    {
        ownerSpawner.OnGhostDied();
        Destroy(gameObject);
    }
}