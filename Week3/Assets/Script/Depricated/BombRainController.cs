using UnityEngine;

public class BombRainController : MonoBehaviour
{
    public GameObject bombPrefab;
    public float spawnHeight = 10f;

    public void DropBombAt(Vector3 targetPosition)
    {
        Vector3 spawnPosition = targetPosition + Vector3.up * spawnHeight;
        GameObject bomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

        // 낙하 타겟 지정
        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            bombScript.SetTargetPosition(targetPosition + Vector3.up * 0.2f);
        }
    }
}
