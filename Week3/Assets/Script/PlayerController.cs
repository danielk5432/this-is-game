using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 3f;

    [Header("상자 들기/놓기")]
    public Transform headPosition;      // 머리 위 위치 (빈 오브젝트)
    public string boxTag = "Box";          // 상자 감지용 레이어
    public float pickupRadius = 1f;
    public float dropOffset = 1f;
    public float boxOffset = 1f;

    [Header("캐릭터 스케일")]
    public float shrinkAmountPerWeight = 0.02f; // 무게 1당 줄어드는 높이
    public float minHeightScale = 0.5f;         // 너무 작아지는 것 방지

    private Vector2 moveInput;
    private Vector2 lookDirection = Vector2.down; // 초기 바라보는 방향
    private Rigidbody2D rb;
    private List<GameObject> carriedBoxes = new List<GameObject>();
    private GameObject carriedBox = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalHeightScale = transform.localScale.y;
    }

    void Update()
    {
        // 입력 처리
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (moveInput != Vector2.zero)
            lookDirection = moveInput; // 이동 방향 = 바라보는 방향

        // 상자 줍기
        if (Input.GetKeyDown(KeyCode.E))
            TryPickupBox();

        // 상자 내려놓기
        if (Input.GetKeyDown(KeyCode.Q))
            DropBox();

        UpdateCarriedBoxesPosition();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void TryPickupBox()
    {
        if (carriedBox != null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(boxTag) && !carriedBoxes.Contains(hit.gameObject))
            {
                GameObject box = hit.gameObject;

                // Rigidbody 비활성화
                var rb2d = box.GetComponent<Rigidbody2D>();
                if (rb2d != null) rb2d.simulated = false;

                // 스택에 추가
                carriedBoxes.Add(box);

                // 무게 반영: 키 줄이기
                float weight = GetBoxWeight(box);
                ShrinkHeight(weight);
                return;
            }
        }
    }

    void DropBox()
    {
        if (carriedBox == null) return;

        Vector2 dropPos = (Vector2)transform.position + lookDirection * dropOffset;
        carriedBox.transform.position = dropPos;

        var rb2d = carriedBox.GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.simulated = true;

        carriedBox = null;

    }
}
