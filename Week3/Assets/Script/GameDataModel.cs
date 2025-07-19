using UnityEngine; 

public class GameDataModel : MonoBehaviour
{
    public static GameDataModel Instance { get; private set; }

    public int totalScore;
    public int unlockedLevels;
    
    // [추가] 다음 씬에서 사용할 스폰 포인트 ID
    public string nextSpawnPointIdentifier;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}