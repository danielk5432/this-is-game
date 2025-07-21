using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform target; // Player

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null)
        {
            Debug.LogError("Player not found");
        }
    }
    void Update()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;

            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetStun();
                Debug.Log("GhostEnemy hit Player");
                Destroy(gameObject);
            }
        }
    }
}
