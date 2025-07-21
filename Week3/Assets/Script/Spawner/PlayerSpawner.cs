using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("생성할 플레이어 프리팹")]
    public GameObject playerPrefab;

    public void SpawnPlayer()
    {
        // 1. 데이터 모델에서 목표 스폰 ID를 가져옴
        string targetSpawnId = GameDataModel.Instance.nextSpawnPointIdentifier;

        SpawnPoint targetSpawnPoint = null;

        // 2. 씬에 있는 모든 스폰 포인트를 찾음
        SpawnPoint[] allSpawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (SpawnPoint spawnPoint in allSpawnPoints)
        {
            if (spawnPoint.spawnIdentifier == targetSpawnId)
            {
                targetSpawnPoint = spawnPoint;
                break; // 목표 지점을 찾았으니 반복 중단
            }
        }


        // 3. 플레이어 생성
        if (targetSpawnPoint != null)
        {
            // 일치하는 스폰 포인트를 찾았으면 그 위치에 생성
            Instantiate(playerPrefab, targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation);
        }
        else
        {
            // 못 찾았거나, 게임을 처음 시작해서 ID가 없는 경우 기본 위치에 생성
            Debug.LogWarning("목표 스폰 포인트를 찾지 못했습니다. 기본 위치에 생성합니다.");
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}