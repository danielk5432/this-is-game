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
    public GameObject healthFrame;
    public GameObject healthBar;
    [Tooltip("The array of sprites to display, in order from full to empty.")]
    public Sprite[] healthBarImages; // Image component for the health bar

    private List<GameObject> spawnedIcons = new List<GameObject>();

    // --- Private State Variables ---
    private List<BoxData> requiredBoxes = new List<BoxData>();
    private List<BoxData> insertedBoxes = new List<BoxData>();
    [Tooltip("Default Time limit in seconds to repair this machine (if time not set from levelControler).")]
    public float defaultTimeToRepair = 60f; // 이 기계를 수리해야 하는 제한 시간
    private Coroutine timerCoroutine; // 실행 중인 타이머 코루틴을 저장할 변수
    private Image healthBarImage; // Image component for the health bar

    private void Awake()
    {
        healthBarImage = healthBar.GetComponent<Image>();
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

    /// <summary>
    /// Called by the LevelController to break this machine.
    /// </summary>
    public void TriggerBreakdown(List<BoxData> newRequiredBoxes, float time = 0f)
    {
        if (currentState == MachineState.NORMAL)
        {
            ChangeState(MachineState.BROKEN);
            requiredBoxes = newRequiredBoxes;
            insertedBoxes.Clear();
            Debug.Log(gameObject.name + " has broken down!");
            // Start the self-destruct timer.
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            defaultTimeToRepair = time > 0 ? time : defaultTimeToRepair; // Use provided time or default
            timerCoroutine = StartCoroutine(TimerRoutine());
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

    private IEnumerator TimerRoutine()
    {
        if(healthFrame != null) healthFrame.gameObject.SetActive(true);
        if(healthBar != null) healthBar.gameObject.SetActive(true);
        float currentTime = defaultTimeToRepair;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if(healthBar != null)
            {
                // 1. Calculate the current time as a percentage (from 1.0 down to 0.0)
            float timePercentage = currentTime / defaultTimeToRepair;

            // 2. Calculate which sprite index to show.
            // We want the last image (e.g., index 5) when time is 0%,
            // and the first image (index 0) when time is 100%.
            int spriteIndex = Mathf.Clamp(
                Mathf.FloorToInt((1 - timePercentage) * healthBarImages.Length), 
                0, 
                healthBarImages.Length - 1
            );

            // 3. Set the Image's sprite to the correct one from the array.
            healthBarImage.sprite = healthBarImages[spriteIndex];
            }
            yield return null;
        }

        // --- GAME OVER TRIGGER ---
        // Time's up! Tell the LevelController it's game over.
        healthBarImage.sprite = healthBarImages[healthBarImages.Length - 1];
        Debug.Log(gameObject.name + " failed to be repaired in time!");
        //BaseLevelController.Instance.TriggerGameOver("Repair time limit exceeded for " + gameObject.name);
    }

    private void Repair()
    {
        Debug.Log(gameObject.name + " has been repaired!");
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        if(healthFrame != null) healthFrame.gameObject.SetActive(false);
        if(healthBar != null) healthBar.gameObject.SetActive(false);
        ChangeState(MachineState.NORMAL);
        requiredBoxes.Clear();
        insertedBoxes.Clear();
        clearEffect?.Play(); // Play the clear effect if assigned
        BaseLevelController.Instance.OnMachineRepaired();
    }
    
}