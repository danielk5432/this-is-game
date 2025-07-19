using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        // GameDataModel과 같은 로직으로 싱글톤 설정
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

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("씬 이름이 지정되지 않았습니다!");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
    
    // 편의 함수
    public void LoadLevelSelectScene()
    {
        LoadScene("LevelSelect");
    }
}