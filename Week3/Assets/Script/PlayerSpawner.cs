using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("생성할 플레이어 프리팹")]
    public GameObject playerPrefab;
    public static Transform playerInstance { get; private set; }

    public void SpawnPlayer()
    {
        // 1. 데이터 모델에서 목표 스폰 ID를 가져옴
        string targetSpawnId = GameDataModel.Instance.nextSpawnPointIdentifier;

        SpawnPoint targetSpawnPoint = null;

        // 2. 씬에 있는 모든 스폰 포인트를 찾음
        SpawnPoint[] allSpawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (SpawnPoint spawnPoint in allSpawnPoints)
        {
            Debug.Log("Checking spawn point: " + spawnPoint.spawnIdentifier + " against target: " + targetSpawnId);
            if (spawnPoint.spawnIdentifier == targetSpawnId)
            {
                targetSpawnPoint = spawnPoint;
                break; // 목표 지점을 찾았으니 반복 중단
            }
        }

        GameObject spawnedPlayer;


        // 3. 플레이어 생성
        if (targetSpawnPoint != null)
        {
            // 일치하는 스폰 포인트를 찾았으면 그 위치에 생성
            spawnedPlayer = Instantiate(playerPrefab, targetSpawnPoint.transform.position, targetSpawnPoint.transform.rotation, targetSpawnPoint.transform.parent);
            spawnedPlayer.layer = targetSpawnPoint.gameObject.layer;

            // 👇 [2] Sorting Layer 설정: PF Player 하위에서 SpriteRenderer 찾아서 설정
            Transform pfPlayer = spawnedPlayer.transform.Find("PF Player");
            if (pfPlayer != null)
            {
                SpriteRenderer sr = pfPlayer.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    string sortingLayerName = LayerMask.LayerToName(targetSpawnPoint.gameObject.layer);
                    sr.sortingLayerName = sortingLayerName;
                    Debug.Log($"PF Player의 sortingLayerName을 \"{sortingLayerName}\"으로 설정함");
                }
                else
                {
                    Debug.LogWarning("PF Player에 SpriteRenderer가 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("PF Player 오브젝트를 찾을 수 없습니다.");
            }

            playerInstance = spawnedPlayer.transform;

            Debug.Log("Player spawned and reference is now available.");
        }
        else
        {
            // 못 찾았거나, 게임을 처음 시작해서 ID가 없는 경우 기본 위치에 생성
            Debug.LogWarning("목표 스폰 포인트를 찾지 못했습니다. 기본 위치에 생성합니다.");
            spawnedPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.target = spawnedPlayer.transform;
        }
        else
        {
            Debug.LogWarning("CameraFollow 스크립트를 Main Camera에 찾을 수 없습니다.");
        }
        
    }
}