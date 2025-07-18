using UnityEngine;

public class CameraAspectRatioScaler : MonoBehaviour
{
    // 목표 화면 비율 (16:9)
    private float targetAspect = 16.0f / 9.0f;

    void Start()
    {
        // 현재 화면 비율
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // 목표 비율보다 화면이 넓은 경우 (좌우 레터박스)
        if (windowAspect > targetAspect)
        {
            float newWidth = targetAspect / windowAspect;
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        // 목표 비율보다 화면이 좁은 경우 (상하 레터박스)
        else
        {
            float newHeight = windowAspect / targetAspect;
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
}