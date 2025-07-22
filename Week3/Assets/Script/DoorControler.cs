using UnityEngine;

public class DoorControler : MonoBehaviour
{
    public GameObject openedDoor; // 문 오브젝트
    public GameObject closedDoor; // 닫힌 문 오브젝트
    public string doorIdentifier = "0"; // 문 식별자

    public void openDoor() // 문 열기
    {
        if (openedDoor != null && closedDoor != null)
        {
            openedDoor.SetActive(true);
            closedDoor.SetActive(false);
            Debug.Log("문이 열렸습니다.");
        }
        else
        {
            Debug.LogError("문 오브젝트가 설정되지 않았습니다.");
        }
    }

    public void closeDoor() // 문 닫기
    {
        if (openedDoor != null && closedDoor != null)
        {
            openedDoor.SetActive(false);
            closedDoor.SetActive(true);
            Debug.Log("문이 닫혔습니다.");
        }
        else
        {
            Debug.LogError("문 오브젝트가 설정되지 않았습니다.");
        }
    }
}
