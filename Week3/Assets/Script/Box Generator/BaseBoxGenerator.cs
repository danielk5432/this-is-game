// BaseBoxGenerator.cs
using UnityEngine;
using System.Collections;

public class BaseBoxGenerator : MonoBehaviour, IInteractable
{
    public enum GeneratorState { IDLE, PRODUCING, COMPLETE }
    public GeneratorState currentState = GeneratorState.IDLE;
    
    public BoxData boxToProduce;
    public bool isBoxRequired = false;
    public BoxData requiredBox;
    public float productionTime = 3.0f;
    public Transform spawnPoint;

    private GameObject producedBoxInstance;

    public bool IsInteractable()
    {
        // Can interact unless it's in the middle of producing.
        return currentState != GeneratorState.PRODUCING;
    }

    public void Interact(PlayerController player)
    {
        if (!IsInteractable()) return;

        switch (currentState)
        {
            case GeneratorState.IDLE:
                BoxData heldBox = player.GetTopBoxData();
                if (isBoxRequired)
                {
                    if (heldBox != null && heldBox.boxID == requiredBox.boxID)
                    {
                        player.ConsumeTopBox(); // Consume the player's box.
                        StartProduction();
                    }
                    else
                    {
                        Debug.Log(requiredBox.boxName + " is required.");
                    }
                }
                else
                {
                    StartProduction();
                }
                break;
            
            case GeneratorState.COMPLETE:
                // Give the produced box to the player.
                player.ReceiveBox(boxToProduce, producedBoxInstance);
                producedBoxInstance = null;
                currentState = GeneratorState.IDLE;
                break;
        }
    }

    private void StartProduction()
    {
        currentState = GeneratorState.PRODUCING;
        StartCoroutine(ProductionRoutine());
    }

    private IEnumerator ProductionRoutine()
    {
        Debug.Log("Producing " + boxToProduce.boxName + "...");
        yield return new WaitForSeconds(productionTime);
        producedBoxInstance = Instantiate(boxToProduce.boxPrefab, spawnPoint.position, spawnPoint.rotation);
        currentState = GeneratorState.COMPLETE;
        Debug.Log("Production complete!");
    }
}