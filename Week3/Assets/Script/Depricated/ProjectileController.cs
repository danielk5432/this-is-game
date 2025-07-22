using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Vector2 direction;
    private float speed;

    public string enemyTag = "Enemy";
    public string boxTag = "Box";
    public string playerTag = "Player";

    public float selfDestructTime = 5f;

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir;
        speed = spd;
    }

    void Start()
    {
        Destroy(gameObject, selfDestructTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            return; // 무시
        }
        else if (other.CompareTag(boxTag))
        {
            BaseBox box = other.GetComponent<BaseBox>();
            if (box != null)
            {
                box.DestroyBox();
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag(playerTag))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.SetStun(2f);
                pc.DropAllBoxes();
            }
            Destroy(gameObject);
        }
        else
        {
            // 벽이나 기타 오브젝트
            Destroy(gameObject);
        }
    }

}
