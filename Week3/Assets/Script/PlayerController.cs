using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Interaction Settings")]
    public Transform headPosition; // The initial stacking point on the player's head.
    public float interactionRadius = 1f;
    public float dropOffset = 1f;

    [Header("Character Scale")]
    public float shrinkAmountPerWeight = 0.02f;
    public float minHeightScale = 0.5f;

    // --- Private Variables ---
    private Vector2 dir;
    private Vector2 lookDirection = Vector2.down;
    private Rigidbody2D rb;
    private readonly List<GameObject> carriedBoxes = new List<GameObject>();
    private float originalHeightScaleY;
    private float originalScaleX;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalHeightScaleY = transform.localScale.y;
        originalScaleX = transform.localScale.x;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleMovementInput();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.Q))
            DropTopBox();

        // The UpdateCarriedBoxesPosition() call is now removed.
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
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
            if (interactable != null)
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
        if (carriedBoxes.Contains(box.gameObject)) return;

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
        GameObject topBox = carriedBoxes[carriedBoxes.Count - 1];
        carriedBoxes.RemoveAt(carriedBoxes.Count - 1);
        
        RestoreHeight(GetBoxWeight(topBox));
        Destroy(topBox);
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
        if (Input.GetKey(KeyCode.A)) { dir.x = -1; lookDirection = Vector2.left; animator.SetInteger("Direction", 3); }
        else if (Input.GetKey(KeyCode.D)) { dir.x = 1; lookDirection = Vector2.right; animator.SetInteger("Direction", 2); }

        if (Input.GetKey(KeyCode.W)) { dir.y = 1; lookDirection = Vector2.up; animator.SetInteger("Direction", 1); }
        else if (Input.GetKey(KeyCode.S)) { dir.y = -1; lookDirection = Vector2.down; animator.SetInteger("Direction", 0); }
        
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
}