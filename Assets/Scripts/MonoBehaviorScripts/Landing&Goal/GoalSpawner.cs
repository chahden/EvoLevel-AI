
using UnityEngine;

/// <summary>
/// Manages the creation and removal of the goal GameObject
/// at a specified grid position.
/// </summary>
public class GoalSpawner : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]

    [Tooltip("Grid manager used to convert grid coordinates into world positions.")]
    [SerializeField] private GridManager _gridManager;

    [Header("Settings")]

    [Tooltip("Settings asset containing the goal prefab.")]
    [SerializeField] private GoalSettings _goalSettings;

    #endregion

    #region Private Variables

    private GameObject _goalInstance;

    #endregion

    #region Properties

    /// <summary>
    /// Indicates whether a goal instance currently exists.
    /// </summary>
    public bool IsGoalSpawned
    {
        get { return _goalInstance != null; }
    }

    #endregion

    #region Goal Spawning

    /// <summary>
    /// Creates the goal at the specified grid cell.
    /// </summary>
    public void SpawnGoal(Vector2Int _goalCell)
    {
        if (_gridManager == null)
        {
            Debug.LogError("GoalSpawner: GridManager reference is missing!", this);
            return;
        }

        if (_goalSettings == null)
        {
            Debug.LogError("GoalSpawner: GoalSettings asset is missing!", this);
            return;
        }

        if (_goalSettings.GoalPrefab == null)
        {
            Debug.LogError("GoalSpawner: Goal prefab is missing from GoalSettings!", this);
            return;
        }

        // Prevent spawning the goal outside the grid boundaries.
        if (!_gridManager.IsInsideGrid(_goalCell))
        {
            Debug.LogError("GoalSpawner: Goal position is outside the grid!", this);
            return;
        }

        // Remove the previous goal instance before creating a new one.
        if (_goalInstance != null)
        {
            Destroy(_goalInstance);
            _goalInstance = null;
        }

        // Convert the requested grid cell into its world-space position.
        Vector2 _spawnPosition = _gridManager.GridToWorld(_goalCell);

        _goalInstance = Instantiate(
            _goalSettings.GoalPrefab,
            _spawnPosition,
            Quaternion.identity
        );

        // Assign a consistent name to the spawned instance.
        _goalInstance.name = "Goal";
    }

    #endregion

    #region Goal Removal

    /// <summary>
    /// Removes the currently spawned goal instance, if one exists.
    /// </summary>
    public void ClearGoal()
    {
        if (_goalInstance != null)
        {
            Destroy(_goalInstance);
            _goalInstance = null;
        }
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Spawning Controller: GoalSpawner
/// Manages the creation and removal of the goal GameObject
/// during level setup or regeneration.
///
/// The Dependencies: GridManager and GoalSettings
/// GridManager provides grid validation and world-position conversion.
/// GoalSettings provides the prefab used to create the goal.
///
/// The Validation Pipeline: SpawnGoal
/// Checks that all required references are available,
/// verifies that the requested goal cell is inside the grid,
/// and removes any previous goal instance before spawning another.
///
/// The Position Conversion Pipeline: GridToWorld
/// Converts the requested grid coordinates into a world-space position
/// so the goal can be instantiated at the center of the corresponding cell.
///
/// The Instantiation Pipeline: Instantiate
/// Creates the goal using the configured prefab and stores
/// the resulting instance for future management.
///
/// The Removal Pipeline: ClearGoal
/// Destroys the current goal instance and resets its reference.
///
/// The Status Interface: IsGoalSpawned
/// Allows other systems to check whether a goal instance exists.
/// </summary>
