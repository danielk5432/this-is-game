using UnityEngine;

public class TestLevelController : BaseLevelController
{
    [Header("Test Level Settings")]
    public GameObject bossPrefab;
    //public Transform bossSpawnPoint;

    // 부모 클래스의 Start() 함수는 자동으로 호출됩니다.
    // (플레이어 생성, 공통 UI 설정 등)

    // 부모의 abstract 함수를 레벨 2에 맞게 구현합니다.
    protected override void InitializeLevel()
    {
        Debug.Log("Setting up level 1...");
        
    }
}