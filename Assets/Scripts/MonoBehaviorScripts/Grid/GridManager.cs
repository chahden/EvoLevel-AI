
using UnityEngine;

/// <summary>
/// Manages grid coordinates, world-to-grid conversions,
/// and boundary checks using the runtime level configuration.
/// </summary>
public class GridManager : MonoBehaviour
{
    #region Inspector Fields

    [Header("Grid Origin")]

    [Tooltip("World position of the bottom-left corner of the grid.")]
    [SerializeField] private Vector2 _originePosition;

    #endregion

    #region Runtime Configuration

    private int _width;
    private int _height;
    private float _cellSize;

    private bool _isInitialized;

    public int Width
    {
        get { return _width; }
    }

    public int Height
    {
        get { return _height; }
    }

    public float CellSize
    {
        get { return _cellSize; }
    }

    public bool IsInitialized
    {
        get { return _isInitialized; }
    }

    #endregion

    #region Initialization

    public bool Initialize(LevelConfiguration _configuration)
    {
        if (_configuration == null)
        {
            Debug.LogError(
                "GridManager: LevelConfiguration is missing!",
                this
            );

            return false;
        }

        bool _isValid = LevelConfigValidator.Validate(
            _configuration,
            out string _errorMessage
        );

        if (!_isValid)
        {
            Debug.LogError(
                "GridManager: Invalid configuration. " + _errorMessage,
                this
            );

            return false;
        }

        _width = _configuration.Width;
        _height = _configuration.Height;
        _cellSize = _configuration.CellSize;

        _isInitialized = true;

        Debug.Log(
            "GridManager initialized successfully. Grid size: "
            + _width + " x " + _height,
            this
        );

        return true;
    }

    #endregion

    #region Grid Conversion Methods

    /// <summary>
    /// Converts a world position into grid coordinates.
    /// </summary>
    public Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        if (!_isInitialized)
        {
            Debug.LogError("GridManager has not been initialized!", this);
            return Vector2Int.zero;
        }

        int _x = Mathf.FloorToInt(
            (worldPosition.x - _originePosition.x) / _cellSize
        );

        int _y = Mathf.FloorToInt(
            (worldPosition.y - _originePosition.y) / _cellSize
        );

        return new Vector2Int(_x, _y);
    }

    /// <summary>
    /// Converts grid coordinates into the world position
    /// at the center of the corresponding cell.
    /// </summary>
    public Vector2 GridToWorld(Vector2Int gridPosition)
    {
        if (!_isInitialized)
        {
            Debug.LogError("GridManager has not been initialized!", this);
            return Vector2.zero;
        }

        float _x = _originePosition.x
            + (gridPosition.x + 0.5f) * _cellSize;

        float _y = _originePosition.y
            + (gridPosition.y + 0.5f) * _cellSize;

        return new Vector2(_x, _y);
    }

    #endregion

    #region Validation Methods

    /// <summary>
    /// Checks whether a grid coordinate is inside the grid boundaries.
    /// </summary>
    public bool IsInsideGrid(Vector2Int gridPosition)
    {
        if (!_isInitialized)
        {
            return false;
        }

        if (gridPosition.x < 0 || gridPosition.x >= _width)
        {
            return false;
        }

        if (gridPosition.y < 0 || gridPosition.y >= _height)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks whether a world position is inside the grid boundaries.
    /// </summary>
    public bool IsInsideGrid(Vector2 worldPosition)
    {
        if (!_isInitialized)
        {
            return false;
        }

        Vector2Int _gridPosition = WorldToGrid(worldPosition);

        return IsInsideGrid(_gridPosition);
    }

    #endregion
}

/// <summary>
/// The Data Source: LevelSettings 
/// The Logic Hub: GridManager
/// It hold the refrence from data Souce then manage to fetch the dimension dynamically
/// The Math Pipeline: WorldToGrid and GridToWorld
/// WorldToGrid take the world position and convert it to grid position and vice versa
/// GridToWorld take the grid position and convert it to world position
/// The Validation Pipeline: IsInsideGrid
/// IsInsideGrid take the grid position or world position and check if it is inside the grid or not
/// </summary>