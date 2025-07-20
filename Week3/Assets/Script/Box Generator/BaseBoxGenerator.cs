using UnityEngine;
using System.Collections;
using System;

// BaseBoxGenerator now implements the IInteractable interface.
public class BaseBoxGenerator : MonoBehaviour, IInteractable
{
    public enum GeneratorState { IDLE, PRODUCING, COMPLETE }

    [Header("State")]
    [Tooltip("The current state of the generator.")]
    public GeneratorState currentState = GeneratorState.IDLE;

    [Header("Box Data")]
    [Tooltip("The data of the box this generator produces.")]
    public BoxData boxToProduce;
    [Tooltip("Does this generator require a specific box to start production?")]
    public bool isBoxRequired = false;
    [Tooltip("The required box data if isBoxRequired is true.")]
    public BoxData requiredBox;

    [Header("Settings")]
    [Tooltip("Time in seconds to produce the box.")]
    public float productionTime = 3.0f;
    [Tooltip("The location where the produced box will spawn.")]
    public Transform spawnPoint;

    // The spawned GameObject instance of the completed box.
    private GameObject producedBoxInstance;

    #region IInteractable Implementation

    /// <summary>
    /// Checks if the generator can be interacted with.
    /// Interaction is disabled only while producing.
    /// </summary>
    public bool IsInteractable()
    {
        return currentState != GeneratorState.PRODUCING;
    }

    /// <summary>
    /// Main interaction point called by the player.
    /// This method is part of the IInteractable interface.
    /// </summary>
    public BoxData OnInteract(BoxData heldBox)
    {
        // The IsInteractable check should be done by the Player/Manager script before calling this.
        // However, as a safety check, we can add it here too.
        if (!IsInteractable())
        {
            Debug.Log("Currently producing...");
            return heldBox;
        }

        switch (currentState)
        {
            case GeneratorState.COMPLETE:
                Debug.Log("Picked up " + boxToProduce.boxName + "!");
                BoxData pickedUpBox = boxToProduce;
                PickUpCompletedBox();
                return pickedUpBox;

            case GeneratorState.IDLE:
                if (isBoxRequired)
                {
                    if (heldBox != null && heldBox.boxID == requiredBox.boxID)
                    {
                        Debug.Log("Used " + heldBox.boxName + " to start production.");
                        StartProduction();
                        return null; // Signals that the held item was consumed.
                    }
                    else
                    {
                        Debug.Log(requiredBox.boxName + " is required to start production.");
                        return heldBox; // No interaction happened.
                    }
                }
                else
                {
                    Debug.Log("Starting production.");
                    StartProduction();
                    return heldBox; // No interaction with items.
                }
        }
        return heldBox; // Default case.
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    #endregion

    private void StartProduction()
    {
        currentState = GeneratorState.PRODUCING;
        StartCoroutine(ProductionRoutine());
    }

    private IEnumerator ProductionRoutine()
    {
        Debug.Log("Producing " + boxToProduce.boxName + "... (" + productionTime + "s)");

        if (boxToProduce == null || boxToProduce.boxPrefab == null)
        {
            Debug.LogError(gameObject.name + ": BoxData to produce or its Prefab is not set!");
            currentState = GeneratorState.IDLE;
            yield break;
        }

        yield return new WaitForSeconds(productionTime);

        Debug.Log("Production complete!");
        currentState = GeneratorState.COMPLETE;
        producedBoxInstance = Instantiate(boxToProduce.boxPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void PickUpCompletedBox()
    {
        if (producedBoxInstance != null)
        {
            Destroy(producedBoxInstance);
        }
        currentState = GeneratorState.IDLE;
    }


    #region Debugging
    private GUIStyle debugLabelStyle;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // --- Initialize the GUIStyle for the debug label ---
        debugLabelStyle = new GUIStyle();
        debugLabelStyle.fontSize = 20; // 👈 글자 크기를 20으로 설정 (원하는 크기로 조절)
        debugLabelStyle.normal.textColor = Color.white; // 글자 색상을 흰색으로 설정
        debugLabelStyle.fontStyle = FontStyle.Bold; // 글자를 굵게 만듦
        debugLabelStyle.alignment = TextAnchor.MiddleCenter; // 텍스트를 중앙 정렬
    }

    // This function is called for rendering and handling GUI events.
    private void OnGUI()
    {
        // Convert the world position of the object to a screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Invert the y-coordinate because GUI and screen coordinates are different
        screenPos.y = Screen.height - screenPos.y;

        // Only draw if the object is in front of the camera
        if (screenPos.z > 0)
        {
            // Define the rectangle for the label
            Rect labelRect = new Rect(screenPos.x - 100, screenPos.y, 200, 40); // 텍스트가 잘리지 않도록 박스 크기도 키워줌

            // Draw the current state as a text label using the new style
            GUI.Label(labelRect, "State: " + Enum.GetName(typeof(GeneratorState), (int)currentState), debugLabelStyle); // 👈 마지막에 스타일 적용
        }
    }
    #endregion
}

