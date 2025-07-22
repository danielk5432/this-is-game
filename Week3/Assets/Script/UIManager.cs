using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요

public class UIManager : MonoBehaviour
{
    // Singleton: 다른 스크립트에서 UIManager.Instance로 쉽게 접근 가능
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI repairProgressText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        repairProgressText.enabled = false;
    }

    /// <summary>
    /// 레벨 이름 텍스트를 설정합니다.
    /// </summary>
    public void SetLevelName(string name)
    {
        if (levelNameText != null)
        {
            levelNameText.text = name;
        }
    }

    /// <summary>
    /// 수리 진행 상황 텍스트를 업데이트합니다. (예: "Repairs: 2 / 5")
    /// </summary>
    public void UpdateRepairProgress(int current, int total)
    {
        if (repairProgressText != null)
        {
            repairProgressText.enabled = true; // 텍스트를 활성화
            repairProgressText.text = "Repairs: " + current + " / " + total;
        }
    }
    public void HideRepairProgress()
    {
        if (repairProgressText != null)
        {
            repairProgressText.enabled = false; // 텍스트를 비활성화
        }
    }
}