using UnityEngine;

[CreateAssetMenu(menuName = "TilePath/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("Use characters: '.' = open, '#' = wall, 'S' = start. All rows must be same length.")]
    public string[] rows;

    public int Width  => rows != null && rows.Length > 0 ? rows[0].Length : 0;
    public int Height => rows != null ? rows.Length : 0;
}