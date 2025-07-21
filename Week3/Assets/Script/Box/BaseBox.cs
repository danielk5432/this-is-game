// BaseBox.cs
using UnityEngine;

public class BaseBox : MonoBehaviour, IInteractable
{
    public BoxData boxData;
    private bool isHeld = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        transform.localPosition = Vector3.zero; // Reset position relative to parent.
    }

    public void OnDrop(Vector2 dropPosition)
    {
        isHeld = false;
        transform.SetParent(null);
        transform.position = dropPosition;
        if (rb != null) rb.simulated = true;
    }
}