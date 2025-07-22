using UnityEngine;

public class BurstEnemyController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float delayBeforeAttack = 1.5f;
    public float projectileSpeed = 6f;

    private bool hasAttacked = false;

    void Start()
    {
        Invoke(nameof(ShootProjectiles), delayBeforeAttack);
    }

    void ShootProjectiles()
    {
        if (hasAttacked) return;
        hasAttacked = true;

        Vector2[] directions = new Vector2[]
        {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(-1, -1).normalized
        };

        foreach (Vector2 dir in directions)
        {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            ProjectileController pc = proj.GetComponent<ProjectileController>();
            pc.Initialize(dir, projectileSpeed);
        }

        Destroy(gameObject);
    }
}
