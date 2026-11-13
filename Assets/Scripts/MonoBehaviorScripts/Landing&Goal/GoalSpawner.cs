
using UnityEngine;

public class GoalSpawner : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]
    [SerializeField] private GridManager _gridManager;

    [Header("Settings")]
    [SerializeField] private GoalSettings _goalSettings;

    #endregion

    #region Private Variables

    private GameObject _goalInstance;

    #endregion

    #region Properties

    public bool IsGoalSpawned
    {
        get { return _goalInstance != null; }
    }

    #endregion

    #region Goal Spawning

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

        if (!_gridManager.IsInsideGrid(_goalCell))
        {
            Debug.LogError("GoalSpawner: Goal position is outside the grid!", this);
            return;
        }

        if (_goalInstance != null)
        {
            Destroy(_goalInstance);
            _goalInstance = null;
        }

        Vector2 _spawnPosition = _gridManager.GridToWorld(_goalCell);

        _goalInstance = Instantiate(
            _goalSettings.GoalPrefab,
            _spawnPosition,
            Quaternion.identity
        );

        _goalInstance.name = "Goal";
    }

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
