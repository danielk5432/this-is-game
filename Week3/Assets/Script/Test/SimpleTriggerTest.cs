using UnityEngine;

public class SimpleTriggerTest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 어떤 태그든 상관없이, 일단 트리거에 들어오면 무조건 로그를 출력
        Debug.Log("!!! TRIGGER SUCCESS !!! - " + other.name + "이 들어왔습니다.");
    }
}