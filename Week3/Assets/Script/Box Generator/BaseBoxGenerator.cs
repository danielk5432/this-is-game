using UnityEngine;
using System.Collections;

public class BaseBoxGenerator : MonoBehaviour, IInteractable
{
    public enum GeneratorState { IDLE, PRODUCING, COMPLETE }
    public GeneratorState currentState = GeneratorState.COMPLETE;

    public BoxData boxToProduce;
    public bool isBoxRequired = false;
    public BoxData requiredBox;
    public float productionTime = 3.0f;

    private GameObject producedBoxInstance;
    private GUIStyle stateLabelStyle; // Style for the on-screen debug label

    // This function is called when the script instance is being loaded.
    private void Awake()
    {
        // Initialize the GUIStyle for the state label to make it readable.
        stateLabelStyle = new GUIStyle();
        stateLabelStyle.fontSize = 16;
        stateLabelStyle.fontStyle = FontStyle.Bold;
        stateLabelStyle.normal.textColor = Color.white;
        stateLabelStyle.alignment = TextAnchor.MiddleCenter;
        FinishProduction();
    }

    // This function is for rendering and handling GUI events. We use it for debugging.
    private void OnGUI()
    {
        // Only show debug info if the object is selected in the editor or for all objects if you remove this check.

        if (UnityEditor.Selection.activeGameObject != gameObject) return;


        if (Camera.main == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Only draw the label if the object is in front of the camera.
        if (screenPos.z > 0)
        {
            // Invert Y-axis for GUI coordinate system.
            screenPos.y = Screen.height - screenPos.y;

            // Define the rectangle area for the label above the object.
            Rect labelRect = new Rect(screenPos.x - 75, screenPos.y - 50, 150, 30);

            // Draw the label with the custom style.
            GUI.Label(labelRect, currentState.ToString(), stateLabelStyle);
        }
    }

    public bool IsInteractable()
    {
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
        FinishProduction();
    }

    private void FinishProduction()
    {
        producedBoxInstance = Instantiate(boxToProduce.boxPrefab, new Vector3(-5000, -5000, 0), Quaternion.identity);
        currentState = GeneratorState.COMPLETE;
        Debug.Log("Production complete!");
    }
}