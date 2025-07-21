using UnityEngine;

public class BombRainController : MonoBehaviour
{
    public GameObject bombPrefab;

    public void DropBombAt(Vector3 position)
    {
        Instantiate(bombPrefab, position, Quaternion.identity);
    }
}
