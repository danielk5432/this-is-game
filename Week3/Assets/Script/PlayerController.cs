using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 4f;
    public float minMoveSpeed = 0.1f;
    public float speedDecreasePerWeight = 0.05f;
    public float speedDecreasePerWeightOver20 = 0.1f;
    public float speedDecreasePerWeightOver30 = 0.2f;

    [Header("Interaction Settings")]
    public Transform headPosition; // The initial stacking point on the player's head.
    public float interactionRadius = 1f;
    public float dropOffset = 1f;

    [Header("Character Scale")]
    public float shrinkAmountPerWeight = 0.02f;
    public float minHeightScale = 0.5f;

    [Header("Stun Settings")]
    public bool isStunned = false;
    public float stunDuration = 3f;

    // --- Private Variables ---
    private Vector2 dir;
    private Vector2 lookDirection = Vector2.down;
    private Rigidbody2D rb;
    private readonly List<GameObject> carriedBoxes = new List<GameObject>();
    private float originalHeightScaleY;
    private float originalScaleX;
    private float currentMoveSpeed;
    private float totalWeight = 0f;
    private Animator animator;
    private float stunTimer = 0f;
    private float pickupCooldown = 0.5f;
    private float lastPickupTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalHeightScaleY = transform.localScale.y;
        originalScaleX = transform.localScale.x;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isStunned)
        {
            Stun();
            return;
        }

        HandleMovementInput();

        UpdateSpeedByWeight();
        GetComponent<Rigidbody2D>().linearVelocity = currentMoveSpeed * dir;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.Q))
            DropTopBox();

        UpdateCarriedBoxesPosition();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + dir * currentMoveSpeed * Time.fixedDeltaTime);
    }

    void TryInteract()
    {
        // Find the closest IInteractable object.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius, 1 << gameObject.layer);
        IInteractable closestInteractable = null;
        float minDistance = float.MaxValue;
        foreach (var hit in hits)
        {
            Debug.Log("Found interactable: " + hit.name);
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null && interactable.IsInteractable())
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }
        
        // If an object is found and is interactable, perform the interaction.
        if (closestInteractable != null && closestInteractable.IsInteractable())
        {
            closestInteractable.Interact(this);
        }
    }
    
    // --- Public methods called by other scripts (like BaseBox and BaseBoxGenerator) ---
    
    public void PickupBox(BaseBox box)
    {
        if (Time.time - lastPickupTime < pickupCooldown) return;

        if (carriedBoxes.Contains(box.gameObject)) return;

        lastPickupTime = Time.time;
        // Determine the parent transform for stacking.
        Transform parentTarget = headPosition;
        if (carriedBoxes.Count > 0)
        {
            // Find the "HeadPoint" of the current top box.
            Transform topBoxHeadPoint = carriedBoxes[carriedBoxes.Count - 1].transform.Find("HeadPoint");
            if (topBoxHeadPoint != null)
            {
                parentTarget = topBoxHeadPoint;
            }
        }
        
        // Add to inventory and update scale.
        carriedBoxes.Add(box.gameObject);
        totalWeight += GetBoxWeight(box.gameObject);
        ShrinkHeight(GetBoxWeight(box.gameObject));
        
        // Tell the box it has been picked up and where to attach.
        box.OnPickup(parentTarget);
    }
    
    public void ReceiveBox(BoxData boxData, GameObject boxInstance)
    {
        if (boxInstance == null) return;
        
        BaseBox boxComponent = boxInstance.GetComponent<BaseBox>();
        if(boxComponent != null)
        {
            // Use the same pickup logic to add the newly created box to the stack.
            PickupBox(boxComponent);
        }
    }
    
    public void ConsumeTopBox()
    {
        if (carriedBoxes.Count == 0) return;
        BaseBox topBox = carriedBoxes[carriedBoxes.Count - 1].GetComponent<BaseBox>();
        carriedBoxes.RemoveAt(carriedBoxes.Count - 1);

        totalWeight -= GetBoxWeight(topBox.gameObject);
        RestoreHeight(GetBoxWeight(topBox.gameObject));

        topBox.DestroyBox();
        
    }
    
    public BoxData GetTopBoxData()
    {
        if (carriedBoxes.Count == 0) return null;
        BaseBox boxComponent = carriedBoxes[carriedBoxes.Count - 1].GetComponent<BaseBox>();
        return boxComponent != null ? boxComponent.boxData : null;
    }
    
    void DropTopBox()
    {
        if (carriedBoxes.Count == 0) return;

        GameObject topBoxObject = carriedBoxes[carriedBoxes.Count - 1];
        carriedBoxes.RemoveAt(carriedBoxes.Count - 1);
        
        totalWeight -= GetBoxWeight(topBoxObject);
        RestoreHeight(GetBoxWeight(topBoxObject));

        BaseBox boxComponent = topBoxObject.GetComponent<BaseBox>();
        if (boxComponent != null)
        {
            Vector2 dropPos = (Vector2)transform.position + lookDirection * dropOffset;
            boxComponent.OnDrop(dropPos);
        }
    }

    // --- Unchanged Methods ---

    void HandleMovementInput()
    {
        dir = Vector2.zero;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { dir.x = -1; lookDirection = Vector2.left; animator.SetInteger("Direction", 3); }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { dir.x = 1; lookDirection = Vector2.right; animator.SetInteger("Direction", 2); }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { dir.y = 1; lookDirection = Vector2.up; animator.SetInteger("Direction", 1); }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { dir.y = -1; lookDirection = Vector2.down; animator.SetInteger("Direction", 0); }
        
        dir.Normalize();
        animator.SetBool("IsMoving", dir.magnitude > 0);
    }

    float GetBoxWeight(GameObject box)
    {
        // This could be simplified to get weight from BoxData in the future.
        var rb2d = box.GetComponent<Rigidbody2D>();
        return rb2d != null ? rb2d.mass : 1f;
    }

    void ShrinkHeight(float weight)
    {
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Max(minHeightScale, scale.y - weight * shrinkAmountPerWeight);
        scale.x = originalScaleX;
        transform.localScale = scale;
    }

    void RestoreHeight(float weight)
    {
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Min(originalHeightScaleY, scale.y + weight * shrinkAmountPerWeight);
        scale.x = originalScaleX;
        transform.localScale = scale;
    }

    void UpdateSpeedByWeight()
    {
        if (totalWeight < 21) 
        {
            currentMoveSpeed = baseMoveSpeed - totalWeight * speedDecreasePerWeight;
        }
        else if (totalWeight < 31)
        {
            currentMoveSpeed = baseMoveSpeed - 20f * speedDecreasePerWeight - (totalWeight - 20f) * speedDecreasePerWeightOver20;
        }
        else
        {
            currentMoveSpeed = baseMoveSpeed - 30f * speedDecreasePerWeight - (totalWeight - 30f) * speedDecreasePerWeightOver30;
        }
        currentMoveSpeed = Mathf.Max(minMoveSpeed, currentMoveSpeed);
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

    public void SetStun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
    }

    void Stun()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            isStunned = false;
        }
        dir = Vector2.zero;
        animator.SetBool("IsMoving", false);
        return;
    }

    public void HitByBomb()
    {
        SetStun(stunDuration);
        DropAllBoxes();
    }

    public void DropAllBoxes()
    {
        while (carriedBoxes.Count > 0)
        {
            ConsumeTopBox();
        }
    }
}