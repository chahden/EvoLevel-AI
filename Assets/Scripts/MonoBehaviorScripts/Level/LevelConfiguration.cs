/// <summary>
/// Holds the runtime configuration of a level,
/// including grid dimensions, cell size, and level positions.
/// </summary>

public class LevelConfiguration
{
    #region Grid Configuration

    private int _width;
    private int _height;
    private float _cellSize;

    #endregion


    #region Level Positions

    private UnityEngine.Vector2Int _startCell;
    private UnityEngine.Vector2Int _goalCell;

    #endregion


    #region Properties

    public int Width { get { return _width; } } //Gets the width of the configured grid.
    public int Height { get { return _height; } } // Gets the height of the configured grid.
    public float CellSize { get { return _cellSize; } } // Gets the size of each grid cell.


    public UnityEngine.Vector2Int StartCell { get { return _startCell; } } // Gets the starting cell position of the level.
    public UnityEngine.Vector2Int GoalCell { get { return _goalCell; } } // Gets the goal cell position of the level.

    #endregion


    #region Constructor
    /// <summary>
    /// Creates a runtime level configuration using the provided parameters.
    /// </summary>

    public LevelConfiguration(
        int _width,
        int _height,
        float _cellSize,
        UnityEngine.Vector2Int _startCell,
        UnityEngine.Vector2Int _goalCell
    )
    {
        this._width = _width;
        this._height = _height;
        this._cellSize = _cellSize;

        this._startCell = _startCell;
        this._goalCell = _goalCell;
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Runtime Data Model: LevelConfiguration
/// Represents the configuration of the current level
/// independently from the original LevelSettings asset.
///
/// The Data Structure: Private Fields
/// Stores the grid dimensions, cell size, starting position,
/// and goal position received during object creation.
///
/// The Access Interface: Properties
/// Provides read-only access to the configuration values,
/// preventing other systems from directly modifying its fields.
///
/// The Initialization Pipeline: Constructor
/// Receives all required level parameters and assigns them
/// when a new LevelConfiguration instance is created.
///
/// The Configuration Relationship:
/// LevelSettings provides the default values, while LevelGenerator
/// uses those values to create the runtime LevelConfiguration.
/// Other systems, such as GridManager and MazeGenerator,
/// can then use the runtime configuration.
/// </summary>
