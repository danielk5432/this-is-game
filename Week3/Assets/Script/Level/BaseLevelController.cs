using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public abstract class BaseLevelController : MonoBehaviour
{
    // A singleton instance to be accessible from other scripts like machines.
    public static BaseLevelController Instance { get; private set; }

    protected enum LevelState { Preparing, Playing, Cleared }
    protected LevelState currentState;

    [Header("Base Level Settings")]
    [Tooltip("A unique ID for this level, e.g., 'Tutorial_1' or 'Forest_Main'")]
    public string levelId;
    [Tooltip("Reference to the Player Spawner.")]
    public PlayerSpawner playerSpawner;
    [Tooltip("The number of machine repairs required to clear this level.")]
    [SerializeField] protected int repairsToClear = 6;

    protected int currentRepairs = 0;

    protected virtual void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        currentState = LevelState.Preparing;
    }

    protected virtual void Start()
    {
        if (GameDataModel.Instance.IsLevelCleared(levelId))
        {
            Debug.Log("Level '" + levelId + "' is already cleared. Skipping level start.");
            // If cleared, just open the doors and do nothing else.
            OnAlreadyCleared();
        }
        // ----------------------
        else
        {
            // If not cleared, start the level normally.
            currentState = LevelState.Preparing;
            SpawnPlayer();
            SetupCommonUI();
            InitializeLevel();
            currentState = LevelState.Playing;
        }
    }

    /// <summary>
    /// Called by machines when they are successfully repaired.
    /// </summary>
    public virtual void OnMachineRepaired()
    {
        if (currentState != LevelState.Playing) return;

        currentRepairs++;
        Debug.Log("Repairs: " + currentRepairs + " / " + repairsToClear);

        // Check for level clear condition.
        if (currentRepairs >= repairsToClear)
        {
            StartCoroutine(LevelClearRoutine());
        }
    }

    private IEnumerator LevelClearRoutine()
    {
        currentState = LevelState.Cleared;
        Debug.Log("LEVEL CLEAR!");

        // --- Clear Sequence ---
        // 1. Stop all machines and enemy spawners. (This will be implemented in child class)
        GameDataModel.Instance.MarkLevelAsCleared(levelId);
        OnLevelClear();

        // 2. Visual effects (camera shake, fade to white).
        // For now, we'll just wait.
        yield return new WaitForSeconds(1.5f); // Simulate effects duration.

        // 3. Open the door/teleporter to the next level.
        OpenExit();
    }

    // --- Methods to be implemented by child classes ---
    protected abstract void InitializeLevel();
    protected abstract void OnLevelClear();
    protected abstract void OpenExit();
    protected virtual void OnAlreadyCleared()
    {
        // By default, just open the exit. Child classes can add more logic.
        OpenExit();
    }

    // --- Common Methods ---
    protected void SpawnPlayer()
    {
        if (playerSpawner != null) playerSpawner.SpawnPlayer();
    }

    protected void SetupCommonUI() { /* Common UI setup logic */ }

    public void TriggerGameOver()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLoseText();
        }
        StartCoroutine(WaitForReload());
    }

    IEnumerator WaitForReload()
    {
        // Wait for a few seconds before loading the next level.
        yield return new WaitForSeconds(2f);
        SceneController.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ShowWinText()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowWinText();
        }
        StartCoroutine(WaitForWinTextDelete());
    }
    IEnumerator WaitForWinTextDelete()
    {
        // Wait for a few seconds before loading the next level.
        yield return new WaitForSeconds(2f);
        UIManager.Instance.HideText();
    }
}