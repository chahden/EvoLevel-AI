
using UnityEngine;

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

    public LevelConfiguration CurrentConfiguration
    {
        get { return _currentConfiguration; }
    }

    #endregion

    #region Unity Lifecycle

    public void InitializeLevel()
    {
        PrepareLevelConfiguration();
    }

    #endregion

    #region Configuration Preparation

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

        // Read default values from the settings asset.
        int _width = _levelSettings.Width;
        int _height = _levelSettings.Height;
        float _cellSize = _levelSettings.CellSize;

        Vector2Int _startCell = _levelSettings.DefaultPosition;
        Vector2Int _goalCell = _levelSettings.DefaultGoalCell;

        // Create the runtime configuration.
        _currentConfiguration = new LevelConfiguration(
            _width,
            _height,
            _cellSize,
            _startCell,
            _goalCell
        );

        // Validate the configuration.
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

        // Initialize GridManager using the validated configuration.
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
