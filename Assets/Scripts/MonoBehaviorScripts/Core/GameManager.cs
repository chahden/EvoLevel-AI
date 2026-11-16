
using UnityEngine;

/// <summary>
/// Manages the global game state.
/// Tracks the game timer, handles win and loss conditions,
/// and provides a singleton instance accessible by other scripts.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton

    /// <summary>
    /// Provides global access to the active GameManager instance.
    /// </summary>
    public static GameManager Instance { get; private set; }

    #endregion

    #region Private Variables

    private bool _gameEnded;
    private float _startTime;

    #endregion

    #region Properties

    /// <summary>
    /// Indicates whether the game has ended.
    /// </summary>
    public bool GameEnded
    {
        get { return _gameEnded; }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the singleton instance.
    /// Destroys this GameObject if another GameManager instance already exists.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Starts the game timer when the game begins.
    /// </summary>
    public void Start()
    {
        _startTime = Time.time;

        Debug.Log("Game started! Timer is running.");
    }

    #endregion

    #region Game State

    /// <summary>
    /// Ends the game with a victory state.
    /// Calculates and displays the time spent reaching the goal.
    /// </summary>
    public void WinGame()
    {
        // Prevent the game from ending more than once.
        if (_gameEnded)
        {
            return;
        }

        _gameEnded = true;

        float _timeSpent = Time.time - _startTime;

        Debug.Log("You Win!");
        Debug.Log($"Time to reach the goal: {_timeSpent:F2} seconds");
    }

    /// <summary>
    /// Ends the game with a loss state.
    /// </summary>
    public void LoseGame()
    {
        // Prevent the game from ending more than once.
        if (_gameEnded)
        {
            return;
        }

        _gameEnded = true;

        Debug.Log("You Lose!");
    }

    /// <summary>
    /// Resets the game-ended state and restarts the timer.
    /// Does not reload the scene or recreate game objects.
    /// </summary>
    public void RestartGameState()
    {
        _gameEnded = false;
        _startTime = Time.time;

        Debug.Log("Game restarted! Timer reset.");
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Global Game Controller: GameManager
/// Manages the overall game state and provides a central access point
/// for systems that need to trigger or check game completion.
///
/// The Singleton Pattern: Instance
/// Provides access to the active GameManager through a static property.
/// Awake ensures that only one active instance is retained.
///
/// The Initialization Pipeline: Awake
/// Checks whether another GameManager instance already exists.
/// If one exists, the duplicate GameObject is destroyed.
///
/// The Timer System:
/// Start records the initial game time using Time.time.
/// WinGame calculates the elapsed time when the player wins.
/// RestartGameState resets the timer for another game attempt.
///
/// The Victory Pipeline: WinGame
/// Checks whether the game has already ended.
/// If not, marks the game as ended and displays the completion time.
///
/// The Defeat Pipeline: LoseGame
/// Checks whether the game has already ended.
/// If not, marks the game as ended and displays the loss message.
///
/// The Reset Pipeline: RestartGameState
/// Clears the game-ended state and records a new starting time.
/// It resets the game state but does not reload the scene.
///
/// The State Interface: GameEnded
/// Allows other systems to check whether the game has ended
/// without directly modifying the private game state.
/// </summary>
