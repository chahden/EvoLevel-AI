using UnityEngine;

/// <summary>
/// Stores the default parameters used to configure a level,
/// including grid dimensions, cell size, and default positions.
/// </summary>

[CreateAssetMenu(fileName = "LevelSettings", menuName = "EvoLevel/LevelSettings")]
public class LevelSettings : ScriptableObject
{
    [Header("Grid Configuration")]

    [Tooltip("Number of cells along the horizontal axis of the grid.")]
    [Min(1)]
    [SerializeField] private int _width = 20;
    
    [Tooltip("Number of cells along the vertical axis of the grid.")]
    [Min(1)]
    [SerializeField] private int _height = 20;

    [Tooltip("Size of each cell in the grid.")]
    [Min(0.1f)]
    [SerializeField] private float _cellSize = 1f;

    [Header("Default Position")]

    [Tooltip("Default starting cell position inside the grid.")]
    [SerializeField] private Vector2Int defaultPosition = new Vector2Int(0, 0);

    [Tooltip("Default goal cell position inside the grid.")]
    [SerializeField] private Vector2Int defaultGoalCell  = new Vector2Int(19, 19);


    public int Width { get { return _width; } }
    public int Height { get { return _height; } }
    public float CellSize { get { return _cellSize; } }
    public Vector2Int DefaultPosition { get { return defaultPosition; } }
    public Vector2Int DefaultGoalCell { get { return defaultGoalCell; } }

}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Data Source: LevelSettings
/// Stores the default level parameters as a ScriptableObject asset,
/// allowing them to be configured directly through the Unity Inspector.
///
/// The Inspector Configuration: Serialized Fields
/// Defines the grid dimensions, cell size, starting position,
/// and goal position using editable Inspector fields.
///
/// The Access Interface: Properties
/// Exposes the stored configuration values through public read-only
/// properties so other systems can retrieve them without directly
/// modifying the private fields.
///
/// The Asset Creation Pipeline: CreateAssetMenu
/// Allows LevelSettings assets to be created directly
/// from the Unity Editor through the Assets/Create menu.
///
/// The Configuration Relationship:
/// LevelGenerator reads the default values from this asset
/// and uses them to create the runtime LevelConfiguration.
/// </summary>
