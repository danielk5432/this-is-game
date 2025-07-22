using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map_North_Controller : BaseLevelController
{
    [Header("Level Specific Settings")]
    //public SpawnManager spawnManager;
    public List<BoxData> possibleRequiredBoxes;
    [Tooltip("The fixed number of parts this level's machines will require.")]
    public int requiredPartsCount = 1; // 레벨별로 고정된 요구 부품 개수
    
    [Header("Breakdown Timings")]
    public float firstBreakdownDelay = 2.0f;
    public float periodicBreakdownTime = 20.0f;
    public float fastBreakdownTime = 3.0f;
    public float randomTimeRange = 2.0f;

    private List<BaseMachineController> machinesInLevel;
    private bool firstRepairDone = false;

    protected override void InitializeLevel()
    {
        // Find all machines in this level.
        machinesInLevel = new List<BaseMachineController>(FindObjectsByType<BaseMachineController>(FindObjectsSortMode.None));
        
        // Start the main game loop for this level.
        StartCoroutine(LevelRoutine());
    }

    public override void OnMachineRepaired()
    {
        base.OnMachineRepaired(); // Call the base class logic to count repairs.

        // If this is the first repair, start spawning enemies.
        if (!firstRepairDone)
        {
            firstRepairDone = true;
            /*
            if (spawnManager != null)
            {
                Debug.Log("First repair complete! Enemies will now spawn.");
                spawnManager.StartSpawning(); // Assuming SpawnManager has this method.
            }
            */
        }
    }

    private IEnumerator LevelRoutine()
    {
        // 1. 레벨 시작 후 첫 번째 장치 고장
        yield return new WaitForSeconds(firstBreakdownDelay);
        if (currentState != LevelState.Playing) yield break;
        BreakRandomMachine();

        // 2. 메인 게임 루프
        float periodicTimer = periodicBreakdownTime; // 일반 고장 타이머

        while (currentState == LevelState.Playing)
        {
            // --- 1초마다 상황을 체크 ---
            yield return new WaitForSeconds(1.0f);

            // 2a. 현재 모든 장치가 수리되었는지 확인
            bool allMachinesRepaired = machinesInLevel.All(m => m.currentState == BaseMachineController.MachineState.NORMAL);

            if (allMachinesRepaired)
            {
                // 모든 장치가 수리되었다면, 즉시 짧은 타이머를 실행하고 새 장치를 고장 냄
                Debug.Log("All machines repaired! Triggering fast breakdown...");
                yield return new WaitForSeconds(fastBreakdownTime); // 2~3초의 짧은 대기
                
                if (currentState != LevelState.Playing) yield break;
                BreakRandomMachine();
                
                // 일반 타이머를 초기화하고 루프의 처음으로 돌아감
                periodicTimer = periodicBreakdownTime;
                continue; 
            }

            // 2b. 고장 난 장치가 하나라도 있다면, 일반 타이머를 진행
            periodicTimer -= 1.0f;

            if (periodicTimer <= 0)
            {
                // 일반 타이머가 다 되면, 고장 조건 확인 후 새 장치를 고장 냄
                int repairsLeft = repairsToClear - currentRepairs;
                int brokenMachineCount = machinesInLevel.Count(m => m.currentState != BaseMachineController.MachineState.NORMAL);

                if (brokenMachineCount < repairsLeft)
                {
                    BreakRandomMachine();
                }
                else
                {
                    Debug.Log("Cannot break a new machine. Broken count (" + brokenMachineCount + ") has reached repairs left (" + repairsLeft + ").");
                }

                // 일반 타이머 초기화
                periodicTimer = periodicBreakdownTime;
            }
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

            // Generate a list of required boxes.
            List<BoxData> requirements = new List<BoxData>();

            // Loop for the fixed number of required parts.
            for (int i = 0; i < requiredPartsCount; i++)
            {
                // Select a completely random box from the possible options, allowing duplicates.
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
    
    protected override void OnLevelClear()
    {
        // Stop all enemies and machines.
        //if (spawnManager != null) spawnManager.StopSpawning();
        // You can add logic here to stop all machine activity.
        //StopAllCoroutines();함수 이름 바꾸기 적 생성 코루틴만 없애라
    }

    protected override void OpenExit()
    {
        Debug.Log("Opening all doors with ID '0'.");
        // Find all DoorControler scripts in the scene.
        DoorControler[] allDoors = FindObjectsByType<DoorControler>(FindObjectsSortMode.None);
        
        // Loop through all found doors.
        foreach (DoorControler door in allDoors)
        {
            Debug.Log("Checking door: " + door.doorIdentifier);
            // If the door's identifier is "0", open it.
            if (door.doorIdentifier == "0")
            {
                door.openDoor();
            }
        }
    }
}