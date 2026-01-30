using UnityEngine;

[CreateAssetMenu(menuName = "TilePath/World Data")]
public class WorldData : ScriptableObject
{
    public string worldName;          // "4x4", "5x5"
    public int gridSize;              // 4 or 5 (optional)
    public LevelData[] levels;        // drag your LevelData assets here
}