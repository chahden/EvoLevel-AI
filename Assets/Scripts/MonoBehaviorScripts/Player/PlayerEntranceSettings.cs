
using UnityEngine;

/// <summary>
/// Stores the configuration values used by PlayerEntranceController
/// to determine the player's starting offset and entrance animation duration.
/// </summary>
[CreateAssetMenu(
    fileName = "PlayerEntranceSettings",
    menuName = "EvoLevel/Player/Entrance Settings"
)]
public class PlayerEntranceSettings : ScriptableObject
{
    #region Entrance Settings

    [Header("Entrance Offset")]

    [Tooltip("Horizontal offset between the player's starting position and landing position.")]
    [SerializeField] private float _entranceOffsetX = 3f;

    [Tooltip("Vertical offset between the player's starting position and landing position.")]
    [SerializeField] private float _entranceOffsetY = 3f;


    [Header("Animation")]

    [Tooltip("Duration of the player's entrance movement in seconds.")]
    [Min(0.1f)]
    [SerializeField] private float _entranceDuration = 1.2f;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the horizontal entrance offset.
    /// </summary>
    public float EntranceOffsetX
    {
        get { return _entranceOffsetX; }
    }

    /// <summary>
    /// Gets the vertical entrance offset.
    /// </summary>
    public float EntranceOffsetY
    {
        get { return _entranceOffsetY; }
    }

    /// <summary>
    /// Gets the duration of the entrance animation.
    /// </summary>
    public float EntranceDuration
    {
        get { return _entranceDuration; }
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Configuration Asset: PlayerEntranceSettings
/// Stores the entrance offsets and animation duration.
/// These values can be configured in the Inspector without changing the controller.
///
/// The Entrance Controller: PlayerEntranceController
/// Coordinates the complete player entrance sequence.
/// It manages the player's Rigidbody2D, movement state, entrance coroutine,
/// and interaction with the landing wave animation.
///
/// The Preparation Pipeline: Awake
/// Retrieves the required components, disables gravity,
/// freezes rotation, and disables player movement.
///
/// The Starting Position: PlayEntrance
/// Calculates the player's initial position using the configured offsets,
/// resets velocity, and starts the entrance coroutine.
///
/// The Movement Pipeline: AnimateEntrance
/// Uses interpolation and SmoothStep to move the player
/// smoothly toward the landing position.
///
/// The Landing Effect: LandingWaveAnimation
/// Plays after the player reaches the destination.
/// Its completion callback determines when the player can move again.
///
/// The Completion Pipeline: OnLandingWaveFinished
/// Enables player movement and updates the entrance completion state.
///
/// The Configuration Relationship:
/// PlayerEntranceController reads its values from PlayerEntranceSettings.
/// This separates entrance behavior from its configurable parameters.
/// </summary>
