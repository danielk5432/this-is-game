using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Used for easy list display

public class BaseMachineController : MonoBehaviour, IInteractable
{
    // Defines the operational state of the machine.
    public enum MachineState { NORMAL, BROKEN, DIAGNOSED }
    
    [Header("State")]
    public MachineState currentState = MachineState.NORMAL;

    // In BaseMachineController.cs

    [Header("Visual Components")]
    [Tooltip("The GameObject holding the 'Normal' state sprite.")]
    public GameObject normalStateObject;

    [Tooltip("The GameObject holding the 'Broken' state sprite.")]
    public GameObject brokenStateObject;

    [Tooltip("The exclamation mark indicator object.")]
    public GameObject exclamationMarkObject;

    [Tooltip("The UI Text component to show the required box count (Use TextMeshPro).")]
public TMPro.TextMeshProUGUI boxCountText; // TextMeshPro를 사용하는 것을 강력히 추천합니다.

    // --- Private State Variables ---
    private List<BoxData> requiredBoxes = new List<BoxData>();
    private List<BoxData> insertedBoxes = new List<BoxData>();
    private GUIStyle stateLabelStyle;

    private void Awake()
    {
        // Initialize the GUIStyle for the state label.
        stateLabelStyle = new GUIStyle();
        stateLabelStyle.fontSize = 16;
        stateLabelStyle.fontStyle = FontStyle.Bold;
        stateLabelStyle.normal.textColor = Color.yellow; // Machine status color
        stateLabelStyle.alignment = TextAnchor.MiddleCenter;
        UpdateVisuals(currentState);
    }

    private void ChangeState(MachineState newState)
    {
        currentState = newState;
        UpdateVisuals(newState);
    }

    private void UpdateVisuals(MachineState state)
    {
        // 먼저 모든 요소를 끈다고 가정하고 시작하면 코드가 깔끔해집니다.
        normalStateObject.SetActive(false);
        brokenStateObject.SetActive(false);
        exclamationMarkObject.SetActive(false);
        if(boxCountText != null) boxCountText.gameObject.SetActive(false);

        // 현재 상태에 맞는 요소만 다시 켭니다.
        switch (state)
        {
            case MachineState.NORMAL:
                normalStateObject.SetActive(true);
                brokenStateObject.SetActive(false);
                exclamationMarkObject.SetActive(false);
                break;

            case MachineState.BROKEN:
                normalStateObject.SetActive(false);
                brokenStateObject.SetActive(true);
                exclamationMarkObject.SetActive(true);
                break;

            case MachineState.DIAGNOSED:
                exclamationMarkObject.SetActive(false);
                brokenStateObject.SetActive(true);
                if (boxCountText != null)
                {
                    boxCountText.gameObject.SetActive(true);
                    // 필요한 박스 개수 / 넣은 박스 개수를 표시
                    boxCountText.text = insertedBoxes.Count + " / " + requiredBoxes.Count;
                }
                break;
        }
    }

    private void OnGUI()
    {
        // Simple GUI for debugging the machine's state and requirements.
        if (Camera.main == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPos.z > 0)
        {
            screenPos.y = Screen.height - screenPos.y;
            // Display current state
            Rect labelRect = new Rect(screenPos.x - 100, screenPos.y - 60, 200, 30);
            GUI.Label(labelRect, "State: " + currentState.ToString(), stateLabelStyle);

            // If diagnosed, display the required boxes
            if (currentState == MachineState.DIAGNOSED)
            {
                string requiredText = "NEEDS: " + string.Join(", ", requiredBoxes.Select(b => b.boxName));
                Rect requiredRect = new Rect(screenPos.x - 100, screenPos.y - 40, 200, 30);
                GUI.Label(requiredRect, requiredText, stateLabelStyle);

                string insertedText = "INSERTED: " + string.Join(", ", insertedBoxes.Select(b => b.boxName));
                Rect insertedRect = new Rect(screenPos.x - 100, screenPos.y - 20, 200, 30);
                GUI.Label(insertedRect, insertedText, stateLabelStyle);
            }
        }
    }

    /// <summary>
    /// Called by the LevelController to break this machine.
    /// </summary>
    public void TriggerBreakdown(List<BoxData> newRequiredBoxes)
    {
        if (currentState == MachineState.NORMAL)
        {
            ChangeState(MachineState.BROKEN);
            requiredBoxes = newRequiredBoxes;
            insertedBoxes.Clear();
            Debug.Log(gameObject.name + " has broken down!");
        }
    }

    // --- IInteractable Implementation ---

    public bool IsInteractable()
    {
        // Can only interact if it's broken or diagnosed.
        return currentState == MachineState.BROKEN || currentState == MachineState.DIAGNOSED;
    }

    public void Interact(PlayerController player)
    {
        if (!IsInteractable()) return;

        switch (currentState)
        {
            case MachineState.BROKEN:
                // Interacting with a broken machine diagnoses it.
                ChangeState(MachineState.DIAGNOSED);
                Debug.Log("Diagnosis complete. Required parts revealed.");
                break;

            case MachineState.DIAGNOSED:
                BoxData heldBox = player.GetTopBoxData();
                if (heldBox != null)
                {
                    // Check if the player is holding the NEXT required box in the sequence.
                    if (requiredBoxes.Count > insertedBoxes.Count && heldBox.boxID == requiredBoxes[insertedBoxes.Count].boxID)
                    {
                        Debug.Log("Correct part inserted: " + heldBox.boxName);
                        insertedBoxes.Add(heldBox);
                        player.ConsumeTopBox();

                        // Check if all parts have been inserted.
                        if (insertedBoxes.Count == requiredBoxes.Count)
                        {
                            Repair();
                        }
                    }
                    else
                    {
                        Debug.Log("Wrong part! Still need: " + requiredBoxes[insertedBoxes.Count].boxName);
                    }
                }
                else
                {
                    Debug.Log("You are not holding any parts.");
                }
                break;
        }
    }

    private void Repair()
    {
        Debug.Log(gameObject.name + " has been repaired!");
        ChangeState(MachineState.NORMAL);
        requiredBoxes.Clear();
        insertedBoxes.Clear();
        // You can add repair visual effects here.
    }
}