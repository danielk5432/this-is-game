using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("카메라가 따라갈 대상")]
    public Transform target;               // 따라갈 대상 (플레이어 등)

    [Header("따라가기 부드러움 정도")]
    public float smoothSpeed = 5f;         // 따라오는 속도 (Lerp에 사용)

    [Header("카메라 이동 제한 범위")]
    public Vector2 minBounds = new Vector2(-100, -100);  // 맵 왼쪽 아래
    public Vector2 maxBounds = new Vector2(100, 100);    // 맵 오른쪽 위

    private Camera cam;
    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        cam = Camera.main;
        halfHeight = cam.orthographicSize;
        halfWidth = cam.aspect * halfHeight;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position;

        // 카메라가 보여주는 영역 내에서만 움직이도록 제한
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z);

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed * Time.deltaTime);
    }
}
