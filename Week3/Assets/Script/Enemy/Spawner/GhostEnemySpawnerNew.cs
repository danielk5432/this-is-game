using UnityEngine;
using System.Collections;

public class GhostEnemySpawnerNew : MonoBehaviour
{
    [Tooltip("The GhostEnemy prefab to spawn.")]
    public GameObject ghostEnemyPrefab;
    [Tooltip("A list of possible points where the ghost can spawn.")]
    public Transform[] spawnPoints;
    [Tooltip("Time to respawn after a ghost dies.")]
    public float respawnDelay = 5f;
    [Tooltip("Initial delay before the first ghost spawns.")]
    public float initialDelay = 3f;
    [Tooltip("Random time added or subtracted from delays.")]
    public float randomTimeRange = 2f;

    private bool isSpawning = false;
    private bool stopSpawning = false;

    /// <summary>
    /// Called by EnemySpawnManager to start the spawning process.
    /// </summary>
    public void BeginSpawning()
    {
        stopSpawning = false;
        if (isSpawning) return;
        
        Debug.Log("Ghost Spawner: Began spawning.");
        StartCoroutine(startRoutine()); // Spawn the initial ghost.
    }

    /// <summary>
    /// Called by EnemySpawnManager to stop the spawning process.
    /// </summary>
    public void StopSpawning()
    {
        Debug.Log("Ghost Spawner: Stopped spawning.");
        stopSpawning = true;
        // Stop any pending respawn routines.
        StopAllCoroutines();
    }

    /// <summary>
    /// Called by a GhostEnemy instance when it dies.
    /// </summary>
    public void OnGhostDied()
    {
        // If the spawner is active, start the respawn process.
        if (stopSpawning) return;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator startRoutine()
    {
        yield return new WaitForSeconds(initialDelay + Random.Range(-randomTimeRange, randomTimeRange));
        isSpawning = true;
        SpawnGhost(); // Spawn the first ghost after the initial delay.
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay + Random.Range(-randomTimeRange, randomTimeRange));
        isSpawning = true;

        // Before spawning, double-check if the spawner has been stopped during the delay.
        if (!stopSpawning)
        {
            SpawnGhost();
        }
    }

    private void SpawnGhost()
    {
        if (stopSpawning) return;
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned to GhostEnemySpawner!");
            return;
        }

        // Find the spawn point farthest from the player.

        Transform playerTransform = PlayerSpawner.playerInstance;
        Transform farthestSpawnPoint = null;
        float maxDistanceSqr = -1f; // Use squared distance for performance comparison.

        if (playerTransform != null)
        {
            // Loop through all available spawn points.
            foreach (Transform spawnPoint in spawnPoints)
            {
                // Calculate the squared distance from the player to the spawn point.
                float distanceSqr = (spawnPoint.position - playerTransform.position).sqrMagnitude;

                // If this point is farther than the farthest one found so far, update it.
                if (distanceSqr > maxDistanceSqr)
                {
                    maxDistanceSqr = distanceSqr;
                    farthestSpawnPoint = spawnPoint;
                }
            }
        }

        // As a fallback in case the player isn't found or something goes wrong,
        // just pick a random one.
        if (farthestSpawnPoint == null && spawnPoints.Length > 0)
        {
            farthestSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        // Now, 'farthestSpawnPoint' holds the transform of the farthest point.
        // You can use it to instantiate the enemy.
        // Ex: Instantiate(ghostEnemyPrefab, farthestSpawnPoint.position, farthestSpawnPoint.rotation);
        
        // 2. Instantiate the ghost.
        GameObject ghostInstance = Instantiate(ghostEnemyPrefab, farthestSpawnPoint.position, farthestSpawnPoint.rotation);
        
        // 3. Get the ghost's script component.
        GhostEnemyNew ghostScript = ghostInstance.GetComponent<GhostEnemyNew>();
        if (ghostScript != null)
        {
            // 4. Tell the new ghost that THIS spawner is its owner.
            ghostScript.Initialize(this);
        }
    }
}