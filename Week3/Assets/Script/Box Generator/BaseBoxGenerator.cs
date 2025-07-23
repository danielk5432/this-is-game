using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BaseBoxGenerator : MonoBehaviour, IInteractable
{
    // A mapping from our enum to the integer values in the Animator.
    // IDLE = 0, PRODUCING = 1, COMPLETE = 2.
    public enum GeneratorState { IDLE, PRODUCING, COMPLETE }
    public GeneratorState currentState = GeneratorState.COMPLETE;

    public BoxData boxToProduce;
    public bool isBoxRequired = false;
    public BoxData requiredBox;
    public float productionTime = 3.0f;

    private GameObject producedBoxInstance;

    [Header("Animation Settings")]
    public float animationLength = 0.8f; // open/close 애니메이션 재생 시간

    [Header("Visual Components")]
    public GameObject boxIconObject;
    public Animator animator;
    public Animator indicator;

    private bool isAnimating = false;

    private void Awake()
    {
        
        if (currentState == GeneratorState.COMPLETE)
        {
            ProduceBoxInstance();
        }
        UpdateVisuals(currentState);
    }

    // This is the only place we need to communicate with the Animator.
    private void ChangeState(GeneratorState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        if (animator != null && !isAnimating && newState != GeneratorState.IDLE)
        {
            animator.SetInteger("State", (int)newState);
            StartCoroutine(AnimationLockRoutine());
        }

        UpdateVisuals(newState);
    }

    // UpdateVisuals no longer plays animations, it just handles GameObjects.
    private void UpdateVisuals(GeneratorState state)
    {
        boxIconObject.SetActive(state == GeneratorState.COMPLETE);

        if (state == GeneratorState.COMPLETE && producedBoxInstance != null)
        {
            producedBoxInstance.SetActive(true);
        }
        if (indicator != null)
            switch (state)
            {
                case GeneratorState.IDLE:
                    indicator.Play("Empty");
                    break;
                case GeneratorState.PRODUCING:
                    indicator.Play("Loading");
                    break;
                case GeneratorState.COMPLETE:
                    indicator.Play("Done");
                    break;
            }
    }

    public bool IsInteractable()
    {
        return currentState != GeneratorState.PRODUCING && !isAnimating;
    }

    public void Interact(PlayerController player)
    {
        if (!IsInteractable() && !isAnimating) return;

        switch (currentState)
        {
            case GeneratorState.IDLE:
                BoxData heldBox = player.GetTopBoxData();
                if (isBoxRequired)
                {
                    if (heldBox != null && heldBox.boxID == requiredBox.boxID)
                    {
                        player.ConsumeTopBox();
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
                player.ReceiveBox(boxToProduce, producedBoxInstance);
                producedBoxInstance = null;
                ChangeState(GeneratorState.IDLE);
                break;
        }
    }

    private void StartProduction()
    {
        ChangeState(GeneratorState.PRODUCING);
        StartCoroutine(ProductionRoutine());
    }

    private IEnumerator ProductionRoutine()
    {
        Debug.Log("Producing " + boxToProduce.boxName + "...");
        yield return new WaitForSeconds(productionTime);
        FinishProduction();
    }

    private void FinishProduction()
    {
        ProduceBoxInstance();
        ChangeState(GeneratorState.COMPLETE);
        Debug.Log("Production complete!");
    }
    private IEnumerator AnimationLockRoutine()
    {
        isAnimating = true;
        yield return new WaitForSeconds(animationLength);
        isAnimating = false;
    }
    private void ProduceBoxInstance()
    {
        producedBoxInstance = Instantiate(boxToProduce.boxPrefab, new Vector3(-5000, -5000, 0), Quaternion.identity, transform.parent);
    }
}