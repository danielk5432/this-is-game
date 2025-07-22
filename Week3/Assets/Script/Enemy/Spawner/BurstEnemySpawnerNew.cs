using UnityEngine;
using System.Collections;
using Pathfinding;

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

    private Vector2 GetValidSpawnPosition(Vector2 center, float radius)
    {
        for (int i = 0; i < 20; i++) // 20번 시도
        {
            Vector2 randomPos = center + (Vector2)Random.insideUnitSphere * radius;

            // A*의 그래프 노드 정보를 가져옴
            GraphNode node = AstarPath.active.GetNearest(randomPos).node;

            // 해당 노드가 '이동 가능한' 영역인지 확인
            if (node.Walkable)
            {
                return (Vector3)node.position; // 노드의 실제 위치를 반환
            }
        }
        return center; // 20번 시도 후에도 못찾으면 그냥 중앙에 생성
    }
}