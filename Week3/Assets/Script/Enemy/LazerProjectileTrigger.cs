using UnityEngine;

public class LaserProjectileTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.DropAllBoxes();
                player.SetStun(1.5f);
            }
        }
    }
}
