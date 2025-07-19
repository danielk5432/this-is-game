using UnityEngine;

public class TestLevelController : MonoBehaviour
{
    private float levelTimer = 0f;
    private bool isLevelActive = true;

    public bool isLevelCleared = true;

   [Tooltip("이 레벨을 클리어하면 활성화될 포털")]
    public Portal nextLevelPortal;

    void Start()
    {
        // 레벨 시작 시에는 다음 포털을 비활성화
        if (nextLevelPortal != null && !isLevelCleared)
        {
            nextLevelPortal.isActive = false;
        }
    }

}