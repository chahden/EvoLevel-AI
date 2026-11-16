using UnityEngine;


/// <summary>
/// Prepares the runtime level configuration from the default settings,
/// validates it, and initializes the GridManager.
/// </summary>
/// 

public class LevelGenerator : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]

    [Tooltip("Settings asset containing the default level configuration.")]
    [SerializeField] private LevelSettings _levelSettings;

    [Tooltip("Grid manager that will receive the validated configuration.")]
    [SerializeField] private GridManager _gridManager;

    #endregion

    #region Runtime Configuration

    private LevelConfiguration _currentConfiguration;

    /// <summary>
    /// Gets the runtime configuration of the current level.
    /// </summary>
    /// 
    public LevelConfiguration CurrentConfiguration
    {
        get { return _currentConfiguration; }
    }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Starts the level configuration preparation process.
    /// </summary>

    public void InitializeLevel()
    {
        PrepareLevelConfiguration();
    }

    #endregion

    #region Configuration Preparation
    
    /// <summary>
    /// Reads the default settings, creates and validates the runtime
    /// configuration, then initializes the GridManager.
    /// </summary>

    private void PrepareLevelConfiguration()
    {
        if (_levelSettings == null)
        {
            Debug.LogError(
                "LevelGenerator: LevelSettings asset is missing!",
                this
            );

            return;
        }

        if (_gridManager == null)
        {
            Debug.LogError(
                "LevelGenerator: GridManager reference is missing!",
                this
            );

            return;
        }

        int _width = _levelSettings.Width;
        int _height = _levelSettings.Height;
        float _cellSize = _levelSettings.CellSize;

        Vector2Int _startCell = _levelSettings.DefaultPosition;
        Vector2Int _goalCell = _levelSettings.DefaultGoalCell;

        _currentConfiguration = new LevelConfiguration(
            _width,
            _height,
            _cellSize,
            _startCell,
            _goalCell
        );

        bool _isValid = LevelConfigValidator.Validate(
            _currentConfiguration,
            out string _errorMessage
        );

        if (!_isValid)
        {
            Debug.LogError(
                "LevelGenerator: Invalid configuration. " + _errorMessage,
                this
            );

            _currentConfiguration = null;
            return;
        }

        bool _isGridInitialized = _gridManager.Initialize(
            _currentConfiguration
        );

        if (!_isGridInitialized)
        {
            Debug.LogError(
                "LevelGenerator: GridManager initialization failed!",
                this
            );

            _currentConfiguration = null;
            return;
        }

        Debug.Log(
            "Level configuration prepared and GridManager initialized successfully!",
            this
        );
    }

    #endregion
}
/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Data Source: LevelSettings
/// Stores the default level parameters, including grid dimensions,
/// cell size, starting position, and goal position.
///
/// The Logic Hub: LevelGenerator
/// Retrieves the default parameters, creates the runtime configuration,
/// validates it, and coordinates the GridManager initialization.
///
/// The Runtime Data: LevelConfiguration
/// Holds the configuration values used by the current level.
/// It separates runtime level data from the original settings asset.
///
/// The Validation Pipeline: LevelConfigValidator
/// Checks whether the runtime configuration contains valid parameters
/// before it is used by the grid system.
///
/// The Initialization Pipeline: GridManager
/// Receives the validated configuration and initializes the grid
/// dimensions and coordinate system.
/// </summary>

