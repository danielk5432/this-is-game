using UnityEngine;

[CreateAssetMenu(fileName = "New BoxData", menuName = "Data/Box Data")]
public class BoxData : ScriptableObject
{
    [Tooltip("A unique ID for this box type, e.g., 'box_wood'")]
    public string boxID;
    
    [Tooltip("The display name of the box.")]
    public string boxName;

    [Tooltip("The weight of the box. Used for physics and player effects.")]
    public float weight = 1f; // 무게 변수 추가

    [TextArea] 
    public string description;
    
    [Tooltip("The UI icon for the box.")]
    public Sprite icon;
    
    [Tooltip("The GameObject prefab that represents this box in the world.")]
    public GameObject boxPrefab;
}