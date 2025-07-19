using UnityEngine;

// MonoBehaviour가 아닌 BaseLevelController를 상속받습니다.
public class TestLevel2 : BaseLevelController
{
    [Header("레벨 2 고유 설정")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    // 부모 클래스의 Start() 함수는 자동으로 호출됩니다.
    // (플레이어 생성, 공통 UI 설정 등)

    // 부모의 abstract 함수를 레벨 2에 맞게 구현합니다.
    protected override void InitializeLevel()
    {
        Debug.Log("레벨 2의 고유 설정을 시작합니다.");
        
        // 레벨 2에만 있는 보스 생성 로직
        //Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
    }
}