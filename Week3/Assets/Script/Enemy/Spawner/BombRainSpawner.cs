using UnityEngine;
using System.Collections;

public class BombRainSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject warningIndicatorPrefab; // 경고 표시기 프리팹
    public GameObject bombPrefab;             // 실제 폭탄 프리팹
    public float spawnInterval = 15f;
    public float initialDelay = 3f;
    public float spawnHeight = 10f;

    private Coroutine runningCoroutine;
    private Transform playerTransform;

    public void BeginSpawning()
    {
        if (runningCoroutine != null) return;
        
        // PlayerSpawner가 플레이어를 생성한 후에 안전하게 참조를 가져옵니다.
        playerTransform = PlayerSpawner.playerInstance;
        if (playerTransform == null)
        {
            Debug.LogError("BombRainSpawner cannot find Player!");
            return;
        }

        Debug.Log("Bomb Rain Spawner: Began spawning.");
        runningCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (runningCoroutine != null)
        {
            Debug.Log("Bomb Rain Spawner: Stopped spawning.");
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // 1. 경고 표시기를 생성합니다.
            GameObject indicatorInstance = Instantiate(warningIndicatorPrefab, playerTransform.position, Quaternion.identity);
            BombWarningIndicatorNew indicatorScript = indicatorInstance.GetComponent<BombWarningIndicatorNew>();

            // 2. 경고 표시기에게 플레이어와 '나 자신(스포너)'을 알려줍니다.
            if (indicatorScript != null)
            {
                indicatorScript.Init(playerTransform, this);
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// 경고 표시기가 자신의 역할이 끝났을 때 이 함수를 호출하여 폭탄 투하를 요청합니다.
    /// </summary>
    public void DropBombAt(Vector3 targetPosition)
    {
        Vector3 spawnPosition = targetPosition + Vector3.up * spawnHeight;
        GameObject bombInstance = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

        BombEnemyController bombScript = bombInstance.GetComponent<BombEnemyController>();
        if (bombScript != null)
        {
            bombScript.SetTargetPosition(targetPosition);
        }
    }
}
