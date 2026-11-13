using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "EvoLevel/LevelSettings")]
public class LevelSettings : ScriptableObject
{
    [Header("Grid Configuration")]
    
    [Min(1)]
    [SerializeField] private int _width = 20;
    
    [Min(1)]
    [SerializeField] private int _height = 20;

    [Min(0.1f)]
    [SerializeField] private float _cellSize = 1f;

    [Header("Default Position")]

    [SerializeField] private Vector2Int defaultPosition = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int defaultGoalCell  = new Vector2Int(19, 19);


    public int Width { get { return _width; } }
    public int Height { get { return _height; } }
    public float CellSize { get { return _cellSize; } }
    public Vector2Int DefaultPosition { get { return defaultPosition; } }
    public Vector2Int DefaultGoalCell { get { return defaultGoalCell; } }

}
