using UnityEngine;

public abstract class BaseLevelController : MonoBehaviour
{
    [Header("공통 설정")]
    public PlayerSpawner playerSpawner; // 플레이어 스포너 참조

    protected virtual void Start()
    {
        // 모든 레벨에서 공통으로 실행될 로직
        SpawnPlayer();
        SetupCommonUI();

        // 각 레벨별 고유 로직을 실행할 수 있도록 가상 함수 호출
        InitializeLevel();
    }

    protected void SpawnPlayer()
    {
        if (playerSpawner != null)
        {
            playerSpawner.SpawnPlayer(); // 스포너에게 생성 요청
        }
    }

    protected void SetupCommonUI()
    {
        // 공통 UI (예: 일시정지 버튼 등) 설정 로직
    }

    // 자식 클래스에서 재정의(override)할 수 있는 가상 함수
    protected abstract void InitializeLevel();
}