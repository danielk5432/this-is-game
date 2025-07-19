using UnityEngine;

public class Portal : MonoBehaviour
{
    [Tooltip("이동할 씬의 이름")]
    public string destinationSceneName;

    [Tooltip("도착 씬에서 찾을 스폰 포인트의 고유 ID")]
    public string destinationSpawnPointIdentifier;

    [Tooltip("포털 활성화 여부")]
    public bool isActive = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("포털 충돌 감지: " + other.name);
        // 포털이 비활성화 상태이거나, 플레이어가 아니면 무시
        if (!isActive || !other.CompareTag("Player"))
        {
            return;
        }

        // 방어 코드: 싱글톤 인스턴스가 로드되었는지 확인합니다.
        if (GameDataModel.Instance == null)
        {
            Debug.LogError("GameDataModel.Instance가 존재하지 않습니다! 씬에 GameDataModel 오브젝트가 있는지 확인해주세요.");
            return;
        }
        if (SceneController.Instance == null)
        {
            Debug.LogError("SceneController.Instance가 존재하지 않습니다! 씬에 SceneController 오브젝트가 있는지 확인해주세요.");
            return;
        }

        // 1. 다음 스폰 위치 정보를 싱글톤 데이터 모델에 저장
        GameDataModel.Instance.nextSpawnPointIdentifier = destinationSpawnPointIdentifier;

        // 2. 씬 컨트롤러를 통해 목적지 씬 로드
        SceneController.Instance.LoadScene(destinationSceneName);
    }
}