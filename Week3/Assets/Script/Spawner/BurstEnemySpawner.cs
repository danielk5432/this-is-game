using UnityEngine;

public class BurstEnemySpawner : MonoBehaviour
{
    public GameObject burstEnemyPrefab;
    public float spawnRadius = 1f;

    public void SpawnNearPlayer(Vector2 playerPosition)
    {
        Vector2 spawnPos = GetValidSpawnPosition(playerPosition, spawnRadius);
        Instantiate(burstEnemyPrefab, spawnPos, Quaternion.identity);
    }

    private Vector2 GetValidSpawnPosition(Vector2 center, float radius)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * radius;
            Vector2 pos = center + randomOffset;

            Collider2D hit = Physics2D.OverlapCircle(pos, 1f);
            if (hit == null || hit.CompareTag("Wall") == false)
            {
                return pos;
            }
        }

        return center;
    }
}
