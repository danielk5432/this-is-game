using UnityEngine;

public class SimpleMoveTest : MonoBehaviour
{
    [Tooltip("플레이어의 이동 속도")]
    public float moveSpeed = 8f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        // Rigidbody 컴포넌트를 미리 찾아 변수에 저장
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. 키보드 입력 받기 (WASD 또는 방향키)
        // Update 함수에서는 매 프레임 입력을 감지하는 것이 좋음
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 이동 방향 벡터 계산
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // 2. 물리 엔진을 이용해 Rigidbody 움직이기
        // 물리 관련 처리는 FixedUpdate에서 하는 것이 안정적
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}