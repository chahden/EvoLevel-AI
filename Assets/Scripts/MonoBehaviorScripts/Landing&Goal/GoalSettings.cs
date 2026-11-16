
using UnityEngine;

/// <summary>
/// Stores the configuration required to spawn the goal,
/// including its prefab reference.
/// </summary>
[CreateAssetMenu(
    fileName = "GoalSettings",
    menuName = "EvoLevel/Goal/Goal Settings"
)]
public class GoalSettings : ScriptableObject
{
    #region Inspector Fields

    [Header("Goal Configuration")]

    [Tooltip("Prefab used to create the goal in the level.")]
    [SerializeField] private GameObject _goalPrefab;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the prefab assigned to the goal.
    /// </summary>
    public GameObject GoalPrefab
    {
        get { return _goalPrefab; }
    }

    #endregion
}



/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Data Source: GoalSettings
/// Stores the configuration data required by the goal system.
///
/// The Inspector Configuration: Goal Prefab
/// Holds a reference to the GameObject prefab that will be
/// instantiated when the goal needs to appear in the level.
///
/// The Access Interface: GoalPrefab
/// Exposes the stored prefab through a public read-only property,
/// allowing other systems to access it without directly modifying it.
///
/// The Asset Creation Pipeline: CreateAssetMenu
/// Allows GoalSettings assets to be created directly
/// from the Unity Editor.
///
/// The Configuration Relationship:
/// GoalSpawner reads the prefab reference from GoalSettings
/// and uses it to instantiate the goal at the requested grid position.
/// </summary>
