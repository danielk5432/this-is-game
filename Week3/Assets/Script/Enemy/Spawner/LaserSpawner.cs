using UnityEngine;
using System.Collections;

public class LaserSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject laserPrefab; // LaserControllerNew가 붙어있는 프리팹
    public float spawnInterval = 7f;
    public float spawnRandomRange = 2f; // 레이저가 생성될 때마다 약간의 랜덤 시간 간격을 추가
    public float initialDelay = 5f;

    [Tooltip("레이저가 생성될 수 있는 영역 (월드 좌표 기준)")]
    public Rect spawnArea = new Rect(-10f, -5f, 20f, 10f);
    
    private Coroutine runningCoroutine;

    public void BeginSpawning()
    {
        if (runningCoroutine != null) return;
        Debug.Log("Laser Spawner: Began spawning.");
        runningCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (runningCoroutine != null)
        {
            Debug.Log("Laser Spawner: Stopped spawning.");
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // 1. 랜덤한 시작점과 끝점을 spawnArea 내에서 결정
            // (예: 왼쪽에서 오른쪽으로, 또는 위에서 아래로 등 자유롭게 변경 가능)
            Vector3 startPos = new Vector3(spawnArea.xMin, Random.Range(spawnArea.yMin, spawnArea.yMax), 0);
            Vector3 endPos = new Vector3(spawnArea.xMax, Random.Range(spawnArea.yMin, spawnArea.yMax), 0);
            Vector3 direction = (endPos - startPos).normalized;

            // 2. 레이저 프리팹을 생성
            GameObject laserInstance = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
            LaserControllerNew laserScript = laserInstance.GetComponent<LaserControllerNew>();

            // 3. 생성된 레이저에게 발사 정보 전달
            if (laserScript != null)
            {
                laserScript.Init(startPos, direction);
            }

            yield return new WaitForSeconds(spawnInterval + Random.Range(-spawnRandomRange, spawnRandomRange));
        }
    }
}