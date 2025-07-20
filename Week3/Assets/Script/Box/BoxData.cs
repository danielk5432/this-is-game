using UnityEngine;

[CreateAssetMenu(fileName = "BoxData", menuName = "Scriptable Objects/BoxData")]
public class BoxData : ScriptableObject
{
    public string boxID;
    public string boxName;
    [TextArea] public string description;
    public Sprite icon;
    public GameObject boxPrefab;
}
