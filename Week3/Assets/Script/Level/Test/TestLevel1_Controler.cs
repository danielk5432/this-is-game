using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Used to find NORMAL machines easily

public class TestLevelController : BaseLevelController
{
    [Header("Machine Settings")]
    [Tooltip("The list of all boxes that can be required in this level.")]
    public List<BoxData> possibleRequiredBoxes;
    [Tooltip("Time between machine breakdowns.")]
    public float breakdownInterval = 10f;
    [Tooltip("Min/Max number of parts a machine can require.")]
    public int minRequiredParts = 1;
    public int maxRequiredParts = 3;

    // This list will be populated with all machines in the scene.
    protected List<BaseMachineController> machinesInLevel;

    protected override void InitializeLevel()
    {
        // Find all machines in the current scene and store them.
        machinesInLevel = new List<BaseMachineController>(FindObjectsByType<BaseMachineController>(FindObjectsSortMode.None));
        
        Debug.Log("Level Initialized. Found " + machinesInLevel.Count + " machines.");

        // Start the breakdown cycle.
        StartCoroutine(BreakdownRoutine());
    }

    private IEnumerator BreakdownRoutine()
    {
        // Wait 3 seconds before the first breakdown.
        yield return new WaitForSeconds(3f);
        BreakRandomMachine();

        // Periodically break another machine.
        while (true)
        {
            yield return new WaitForSeconds(breakdownInterval);
            BreakRandomMachine();
        }
    }

    private void BreakRandomMachine()
    {
        // Find all machines that are currently in the NORMAL state.
        var availableMachines = machinesInLevel.Where(m => m.currentState == BaseMachineController.MachineState.NORMAL).ToList();

        if (availableMachines.Count > 0)
        {
            BaseMachineController machineToBreak = availableMachines[Random.Range(0, availableMachines.Count)];
            List<BoxData> requirements = new List<BoxData>();
            int requiredCount = Random.Range(minRequiredParts, maxRequiredParts + 1);

            // --- SELECTION LOGIC CHANGED (TO AVOID DUPLICATES) ---
            // Create a temporary pool of available boxes to pick from.
            List<BoxData> availableBoxesPool = new List<BoxData>(possibleRequiredBoxes);

            for (int i = 0; i < requiredCount; i++)
            {
                // If the pool is empty (e.g., asking for 4 items when there are only 3 types),
                // stop trying to add more requirements.
                if (availableBoxesPool.Count == 0) break;

                // Pick a random box from the pool.
                int randomIndex = Random.Range(0, availableBoxesPool.Count);
                BoxData selectedBox = availableBoxesPool[randomIndex];
                
                // Add it to the requirements and remove it from the pool to prevent re-selection.
                requirements.Add(selectedBox);
                availableBoxesPool.RemoveAt(randomIndex);
            }

            // Tell the machine to break with the generated requirements.
            machineToBreak.TriggerBreakdown(requirements);
        }
        else
        {
            Debug.Log("No available machines to break!");
        }
    }
}