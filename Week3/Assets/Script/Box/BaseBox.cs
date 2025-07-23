// BaseBox.cs
using UnityEngine;
using Pathfinding;

public class BaseBox : MonoBehaviour, IInteractable
{
    public BoxData boxData;
    private bool isHeld = false;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    public GameObject boxBreakParticle;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    public bool IsInteractable()
    {
        // Not held = on the ground = interactable.
        return !isHeld;
    }

    public void Interact(PlayerController player)
    {
        // If interactable, ask the player to pick this box up.
        if (IsInteractable())
        {
            player.PickupBox(this);
        }
    }

    public void OnPickup(Transform parent)
    {
        isHeld = true;
        if (rb != null) rb.simulated = false;
        transform.SetParent(parent);
        // transform.localPosition = Vector3.zero; // Reset position relative to parent.
    }

    public void OnDrop(Vector2 dropPosition)
    {
        // isHeld = false;
        // transform.SetParent(null);
        // transform.position = dropPosition;
        // transform.localScale = originalScale; // Reset scale to original.
        // if (rb != null) rb.simulated = true;

        isHeld = false;
        transform.SetParent(null);

        // 🔍 A* 그래프에서 가장 가까운 노드 가져오기
        GraphNode node = AstarPath.active.GetNearest(dropPosition).node;

        // ✅ 1. 유효한 노드이고, Walkable인 경우 → 그대로 놓기
        if (node != null && node.Walkable)
        {
            transform.position = dropPosition;
        }
        else
        {
            NNConstraint constraint = NNConstraint.Default;
            constraint.constrainWalkability = true;
            constraint.walkable = true;

            var nearest = AstarPath.active.GetNearest(dropPosition, constraint);
            if (nearest.node != null)
            {
                Vector3 safePos = (Vector3)nearest.node.position;
                transform.position = safePos;
                Debug.LogWarning("벽 위에 떨어뜨려서 가장 가까운 안전 지점으로 스냅했습니다.");
            }
            else
            {
                Debug.LogError("안전한 위치를 찾지 못했습니다. 원래 위치로 유지.");
            }
        }

        transform.localScale = originalScale;
        if (rb != null) rb.simulated = true;
    }

    public void DestroyBox()
    {
        Instantiate(boxBreakParticle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}