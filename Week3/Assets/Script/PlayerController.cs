using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float baseMoveSpeed = 4f;
    public float minMoveSpeed = 0.1f;
    public float speedDecreasePerWeight = 0.2f;

    [Header("상자 들기/놓기")]
    public Transform headPosition;           // 머리 위 위치
    public string boxTag = "Box";            // 상자 Tag
    public float pickupRadius = 1f;
    public float dropOffset = 1f;

    [Header("캐릭터 스케일")]
    public float shrinkAmountPerWeight = 0.02f;
    public float minHeightScale = 0.5f;

    private Vector2 dir;
    private Vector2 lookDirection = Vector2.down;
    private Rigidbody2D rb;
    private List<GameObject> carriedBoxes = new List<GameObject>();
    private float originalHeightScaleY;
    private float originalScaleX;
    private float currentMoveSpeed;
    private Animator animator;
    private float totalWeight = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalHeightScaleY = transform.localScale.y;
        originalScaleX = transform.localScale.x;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        dir = Vector2.zero;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            dir.x = -1;
            lookDirection = Vector2.left;
            animator.SetInteger("Direction", 3);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            dir.x = 1;
            lookDirection = Vector2.right;
            animator.SetInteger("Direction", 2);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir.y = 1;
            lookDirection = Vector2.up;
            animator.SetInteger("Direction", 1);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            dir.y = -1;
            lookDirection = Vector2.down;
            animator.SetInteger("Direction", 0);
        }

        dir.Normalize();
        animator.SetBool("IsMoving", dir.magnitude > 0);


        UpdateSpeedByWeight();
        GetComponent<Rigidbody2D>().linearVelocity = currentMoveSpeed * dir;
        // 줍기
        if (Input.GetKeyDown(KeyCode.E))
            TryPickupBox();

        // 내려놓기
        if (Input.GetKeyDown(KeyCode.Q))
            DropTopBox();

        UpdateCarriedBoxesPosition();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + dir * currentMoveSpeed * Time.fixedDeltaTime);
    }

    void TryPickupBox()
    {
        if (carriedBoxes.Count >= 4) return;

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

                // 무게 → 키 줄이기
                float weight = GetBoxWeight(box);
                totalWeight += weight;
                ShrinkHeight(weight);
                return;
            }
        }
    }

    void DropTopBox()
    {
        if (carriedBoxes.Count == 0) return;

        GameObject topBox = carriedBoxes[carriedBoxes.Count - 1];
        carriedBoxes.RemoveAt(carriedBoxes.Count - 1);

        // 내려놓을 위치
        Vector2 dropPos = (Vector2)transform.position + lookDirection * dropOffset;
        topBox.transform.position = dropPos;

        // Rigidbody 다시 켜기
        var rb2d = topBox.GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.simulated = true;

        // 키 복구
        float weight = GetBoxWeight(topBox);
        totalWeight -= weight;
        RestoreHeight(weight);
    }

    void UpdateCarriedBoxesPosition()
    {
        Vector3 currentPos = headPosition.position;
        float baseZ = transform.position.z;

        for (int i = 0; i < carriedBoxes.Count; i++)
        {
            GameObject box = carriedBoxes[i];
            Transform headPoint = box.transform.Find("HeadPoint");

            if (i == 0)
            {
                Vector3 newPos = currentPos;
                newPos.z = baseZ - 1f - (i * 1f);
                box.transform.position = newPos;
                continue;
            }

            if (headPoint != null)
            {
                // HeadPoint가 Box에서 얼마나 떨어져 있는지 (로컬 오프셋 기준)
                Vector3 localOffset = headPoint.position - box.transform.position;

                // 위치 = 현재 쌓인 기준점 + offset
                Vector3 newPos = currentPos + localOffset;

                // 위에 갈수록 더 앞으로
                newPos.z = baseZ - 1f - (i * 1f);

                // 적용
                box.transform.position = newPos;

                // 다음 박스를 쌓을 기준 위치는 현재 상자의 HeadPoint
                currentPos = newPos;
            }
            else
            {
                Debug.LogWarning("HeadPoint가 설정되지 않은 상자가 있음: " + box.name);
            }
        }
    }

    float GetBoxWeight(GameObject box)
    {
        var rb2d = box.GetComponent<Rigidbody2D>();
        return rb2d != null ? rb2d.mass : 1f;
    }

    void ShrinkHeight(float weight)
    {
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Max(minHeightScale, scale.y - weight * shrinkAmountPerWeight);
        scale.x = originalScaleX; // X축은 유지
        transform.localScale = scale;
    }

    void RestoreHeight(float weight)
    {
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Min(originalHeightScaleY, scale.y + weight * shrinkAmountPerWeight);
        scale.x = originalScaleX; // X축은 유지
        transform.localScale = scale;
    }

    void UpdateSpeedByWeight()
    {
        currentMoveSpeed = baseMoveSpeed - totalWeight * speedDecreasePerWeight;
        currentMoveSpeed = Mathf.Max(minMoveSpeed, currentMoveSpeed);
    }
}
