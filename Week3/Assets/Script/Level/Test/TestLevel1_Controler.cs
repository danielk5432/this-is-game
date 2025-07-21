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
            // Pick one random machine from the available list.
            BaseMachineController machineToBreak = availableMachines[Random.Range(0, availableMachines.Count)];

            // Randomly generate a list of required boxes.
            List<BoxData> requirements = new List<BoxData>();
            int requiredCount = Random.Range(minRequiredParts, maxRequiredParts + 1);

            for (int i = 0; i < requiredCount; i++)
            {
                requirements.Add(possibleRequiredBoxes[Random.Range(0, possibleRequiredBoxes.Count)]);
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