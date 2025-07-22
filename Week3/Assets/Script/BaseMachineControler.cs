using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Used for easy list display
using UnityEngine.UI; // For UI components like Image

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
    [Tooltip("Optional particle effect for visual feedback when the machine is repaired.")]
    public ParticleSystem clearEffect; // Optional particle effect for visual feedback

    [Tooltip("The parent object that will hold the required icons.")]
    public Transform iconContainer; // 👈 아이콘들을 담을 컨테이너
    
    [Tooltip("The prefab for a single UI icon.")]
    public GameObject iconPrefab; // 👈 아이콘 UI 프리팹
    
    private List<GameObject> spawnedIcons = new List<GameObject>();

    // --- Private State Variables ---
    private List<BoxData> requiredBoxes = new List<BoxData>();
    private List<BoxData> insertedBoxes = new List<BoxData>();
    private void Awake()
    {
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
        foreach (var icon in spawnedIcons)
        {
            Destroy(icon);
        }
        spawnedIcons.Clear();
        

        // 현재 상태에 맞는 요소만 다시 켭니다.
        switch (state)
        {
            case MachineState.NORMAL:
                normalStateObject.SetActive(true);
                break;

            case MachineState.BROKEN:
                brokenStateObject.SetActive(true);
                exclamationMarkObject.SetActive(true);
                break;

            case MachineState.DIAGNOSED:
                brokenStateObject.SetActive(true);
                var tempInserted = new List<BoxData>(insertedBoxes);
                foreach (var required in requiredBoxes)
                {
                    GameObject iconInstance = Instantiate(iconPrefab, iconContainer);
                    Image iconImage = iconInstance.GetComponent<Image>();
                    iconImage.sprite = required.icon;

                    // Check if this required box has a match in the temporary inserted list.
                    if (tempInserted.Contains(required))
                    {
                        iconImage.color = new Color(0.3f, 0.3f, 0.3f); // Darken if inserted
                        tempInserted.Remove(required); // Remove to handle duplicates correctly
                    }
                    else
                    {
                        iconImage.color = Color.white;
                    }
                    spawnedIcons.Add(iconInstance);
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

            // If diagnosed, display the required boxes
            if (currentState == MachineState.DIAGNOSED)
            {
                string requiredText = "NEEDS: " + string.Join(", ", requiredBoxes.Select(b => b.boxName));
                Rect requiredRect = new Rect(screenPos.x - 100, screenPos.y - 40, 200, 30);

                string insertedText = "INSERTED: " + string.Join(", ", insertedBoxes.Select(b => b.boxName));
                Rect insertedRect = new Rect(screenPos.x - 100, screenPos.y - 20, 200, 30);
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
                    // Count how many of this box type are required.
                    int requiredCount = requiredBoxes.Count(box => box.boxID == heldBox.boxID);
                    // Count how many have already been inserted.
                    int insertedCount = insertedBoxes.Count(box => box.boxID == heldBox.boxID);

                    // If we still need more of this box type, accept it.
                    if (insertedCount < requiredCount)
                    {
                        Debug.Log("Correct part inserted: " + heldBox.boxName);
                        insertedBoxes.Add(heldBox);
                        player.ConsumeTopBox();
                        
                        // --- UPDATE VISUALS IMMEDIATELY ---
                        UpdateVisuals(currentState); // Refresh icons to show the newly inserted box.

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
        clearEffect?.Play(); // Play the clear effect if assigned
        BaseLevelController.Instance.OnMachineRepaired();
    }
}