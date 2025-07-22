using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour
{
    public GameObject warningLine;        // 얇은 선 (경고)
    public GameObject laserProjectile;    // 굵은 레이저 (충돌용)
    public float warningTime = 1f;
    public float laserDuration = 2f;

    private Vector3 direction;
    private Vector3 startPos;
    private Vector3 endPos;

    public void Init(Vector3 start, Vector3 dir)
    {
        direction = dir.normalized;
        startPos = start;
        endPos = start + direction * 30f; // 레이저 길이

        transform.position = Vector3.zero; // 부모는 기준점 0,0 (자식으로 조절)

        SetupWarningLine();
        SetupLaserProjectile();

        StartCoroutine(HandleLaser());
    }

    void SetupWarningLine()
    {
        // 위치와 회전 계산
        float distance = Vector3.Distance(startPos, endPos);
        Vector3 midPoint = (startPos + endPos) / 2f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 세팅
        warningLine.SetActive(true);
        warningLine.transform.position = midPoint;
        warningLine.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        warningLine.transform.localScale = new Vector3(distance, warningLine.transform.localScale.y, 1f);
    }

    void SetupLaserProjectile()
    {
        laserProjectile.SetActive(false);

        float distance = Vector3.Distance(startPos, endPos);
        Vector3 midPoint = (startPos + endPos) / 2f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        laserProjectile.transform.position = midPoint;
        laserProjectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        laserProjectile.transform.localScale = new Vector3(distance, laserProjectile.transform.localScale.y, 1f);
    }

    IEnumerator HandleLaser()
    {
        // 경고만 먼저 보여줌
        yield return new WaitForSeconds(warningTime);

        // 경고 끄고 실제 레이저 표시 + 충돌 감지 시작
        warningLine.SetActive(false);
        laserProjectile.SetActive(true);

        // laserDuration 후 제거
        yield return new WaitForSeconds(laserDuration);

        Destroy(gameObject);
    }
}
