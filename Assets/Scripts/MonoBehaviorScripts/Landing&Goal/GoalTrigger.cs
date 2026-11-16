
using UnityEngine;

/// <summary>
/// Detects when the player reaches the goal
/// and triggers the game completion sequence.
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    #region Trigger Detection

    /// <summary>
    /// Called when another Collider2D enters the goal's trigger area.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check whether the entering object has a PlayerMovement component.
        PlayerMovement _player = other.GetComponent<PlayerMovement>();

        // Complete the game only when the player reaches the goal
        // and the GameManager instance is available.
        if (_player != null && GameManager.Instance != null)
        {
            GameManager.Instance.WinGame();
        }
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Trigger Controller: GoalTrigger
/// Detects when another object enters the goal's trigger area.
///
/// The Detection Pipeline: OnTriggerEnter2D
/// Uses Unity's 2D physics system to detect collisions
/// with the goal's trigger collider.
///
/// The Player Identification: PlayerMovement
/// Checks whether the entering object has a PlayerMovement component
/// to ensure that the goal is activated by the player.
///
/// The Game Completion Pipeline: GameManager
/// Calls WinGame() when the player reaches the goal
/// and the GameManager instance is available.
/// </summary>
