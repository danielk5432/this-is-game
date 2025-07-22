using UnityEngine;
using System.Collections;

public class BurstEnemySpawnerNew : MonoBehaviour
{
    [Header("Settings")]
    public GameObject burstEnemyPrefab;
    public float spawnInterval = 15f;
    public float spawnRadius = 5f;
    public float initialDelay = 3f;

    private Coroutine runningCoroutine;
    private Transform playerTransform;

    /// <summary>
    /// Called by EnemySpawnManager to start the spawning process.
    /// </summary>
    public void BeginSpawning()
    {
        playerTransform = PlayerSpawner.playerInstance;
        
        if (playerTransform == null)
        {
            Debug.LogError("BurstEnemySpawner could not find the player instance from PlayerSpawner!");
        }
        if (runningCoroutine != null) return;
        
        Debug.Log("Burst Spawner: Began spawning.");
        runningCoroutine = StartCoroutine(SpawnLoop());
    }
    /// <summary>
    /// Called by EnemySpawnManager to stop the spawning process.
    /// </summary>
    public void StopSpawning()
    {
        if (runningCoroutine != null)
        {
            Debug.Log("Burst Spawner: Stopped spawning.");
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (playerTransform != null)
            {
                Vector2 spawnPos = GetValidSpawnPosition(playerTransform.position, spawnRadius);
                Instantiate(burstEnemyPrefab, spawnPos, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // This is your original, effective logic for finding a spawn position.
    private Vector2 GetValidSpawnPosition(Vector2 center, float radius)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * radius;
            Vector2 pos = center + randomOffset;

            // Check a small radius to ensure we don't spawn inside something.
            Collider2D hit = Physics2D.OverlapCircle(pos, 0.5f); 
            if (hit == null)
            {
                return pos;
            }
        }
        // Fallback to the center if no valid position is found after 20 tries.
        return center;
    }
}