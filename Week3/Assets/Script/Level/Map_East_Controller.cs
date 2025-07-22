using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map_East_Controller : BaseLevelController
{
    [Header("Level Specific Settings")]
    //public SpawnManager spawnManager;
    public List<BoxData> possibleRequiredBoxes;
    public EnemySpawnManager spawnManager;
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
            spawnManager.StartSpawning(EnemySpawnManager.SpawnType.Ghost);
        }
        else if (currentRepairs == 2)
        {
            requiredPartsCount = 2;
            spawnManager.StartSpawning(EnemySpawnManager.SpawnType.BombRain);
        }
        else if (currentRepairs == 3)
        {
            spawnManager.StartSpawning(EnemySpawnManager.SpawnType.Burst);
        }
        else if (currentRepairs == 4)
        {
            requiredPartsCount = 3;
        }
        else if (currentRepairs == 5)
        {
            spawnManager.StartSpawning(EnemySpawnManager.SpawnType.Burst);
        }
        else if (currentRepairs == 6)
        {
            spawnManager.StartSpawning(EnemySpawnManager.SpawnType.BombRain);
        }
    }
    
    private void SpawnEnemy()
    {
        if (spawnManager != null)
        {
            Debug.Log("First repair complete! Enemies will now spawn.");
            spawnManager.StartSpawning(EnemySpawnManager.SpawnType.Ghost);
            spawnManager.StartSpawning(EnemySpawnManager.SpawnType.Burst); 
            spawnManager.StartSpawning(EnemySpawnManager.SpawnType.BombRain);
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
            BaseMachineController machineToBreak = availableMachines[Random.Range(0, availableMachines.Count)];
            List<BoxData> requirements = new List<BoxData>();

            // --- SELECTION LOGIC CHANGED (TO AVOID DUPLICATES) ---
            // Create a temporary pool of available boxes to pick from.
            List<BoxData> availableBoxesPool = new List<BoxData>(possibleRequiredBoxes);

            for (int i = 0; i < requiredPartsCount; i++)
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