using UnityEngine;
using System.Collections.Generic;

public class GameDataModel : MonoBehaviour
{
    public static GameDataModel Instance { get; private set; }
    public bool isDebugMode = false; // 디버그 모드 여부
    public int totalScore;
    public int unlockedLevels;
    public string nextSpawnPointIdentifier;
    private HashSet<string> clearedLevelIds = new HashSet<string>();

    private void Awake()
    {
        nextSpawnPointIdentifier = "0"; // 초기값 설정

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

    public void MarkLevelAsCleared(string levelId)
    {
        if (!clearedLevelIds.Contains(levelId))
        {
            clearedLevelIds.Add(levelId);
            Debug.Log("Level Cleared and Saved: " + levelId);
        }
    }
    public bool IsLevelCleared(string levelId)
    {
        return clearedLevelIds.Contains(levelId);
    }
}