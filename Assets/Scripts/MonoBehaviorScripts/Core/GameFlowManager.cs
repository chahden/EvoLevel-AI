
using System.Collections;
using UnityEngine;

/// <summary>
/// Coordinates the main level preparation sequence.
/// Controls the order in which the level is initialized,
/// the maze is generated, the player enters, and the goal is spawned.
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]

    [Tooltip("Generates and initializes the current level configuration.")]
    [SerializeField] private LevelGenerator _levelGenerator;

    [Tooltip("Generates the maze and controls its reveal animation.")]
    [SerializeField] private MazeGenerator _mazeGenerator;

    [Tooltip("Creates the player and manages the player's entrance sequence.")]
    [SerializeField] private PlayerSpawner _playerSpawner;

    [Tooltip("Creates the goal at the configured goal cell.")]
    [SerializeField] private GoalSpawner _goalSpawner;

    #endregion

    #region Private Variables

    // Prevents multiple game flow sequences from running simultaneously.
    private Coroutine _gameFlowCoroutine;

    #endregion

    #region Initialization

    /// <summary>
    /// Automatically starts the level preparation sequence
    /// when the scene begins.
    /// </summary>
    private void Start()
    {
        StartGameFlow();
    }

    #endregion

    #region Game Flow

    /// <summary>
    /// Starts the game flow coroutine if another sequence
    /// is not already running.
    /// </summary>
    public void StartGameFlow()
    {
        if (_gameFlowCoroutine != null)
        {
            Debug.LogWarning("Game flow is already running.", this);
            return;
        }

        _gameFlowCoroutine = StartCoroutine(RunGameFlow());
    }

    /// <summary>
    /// Executes the level preparation sequence in order.
    /// Waits for the maze reveal and player entrance to finish
    /// before proceeding to the next stage.
    /// </summary>
    private IEnumerator RunGameFlow()
    {
        // STEP 1: Check LevelGenerator reference.

        if (_levelGenerator == null)
        {
            Debug.LogError("LevelGenerator reference is missing.", this);
            _gameFlowCoroutine = null;
            yield break;
        }

        // STEP 2: Initialize level configuration.

        _levelGenerator.InitializeLevel();

        // STEP 3: Check configuration AFTER initialization.

        if (_levelGenerator.CurrentConfiguration == null)
        {
            Debug.LogError("Level configuration is missing or invalid.", this);
            _gameFlowCoroutine = null;
            yield break;
        }

        // Continue with your existing validation and game sequence...{

        // STEP 1: Validate references and configuration.

        if (_levelGenerator == null ||
            _levelGenerator.CurrentConfiguration == null)
        {
            Debug.LogError("Level configuration is missing or invalid.", this);
            _gameFlowCoroutine = null;
            yield break;
        }

        _levelGenerator.InitializeLevel();

        if (_levelGenerator.CurrentConfiguration == null)
        {
            Debug.LogError("Level initialization failed.", this);
            _gameFlowCoroutine = null;
            yield break;
        }

        if (_mazeGenerator == null ||
            _playerSpawner == null ||
            _goalSpawner == null)
        {
            Debug.LogError("A GameFlowManager reference is missing.", this);
            _gameFlowCoroutine = null;
            yield break;
        }

        LevelConfiguration _configuration =
            _levelGenerator.CurrentConfiguration;

        // STEP 2: Generate the maze.

        _mazeGenerator.GenerateMaze();

        if (!_mazeGenerator.IsGenerated)
        {
            Debug.LogError("Maze generation failed.", this);
            _gameFlowCoroutine = null;
            yield break;
        }

        // Wait until the maze's progressive reveal animation finishes.
        while (!_mazeGenerator.IsRevealComplete)
        {
            yield return null;
        }

        Debug.Log("Maze sequence completed.", this);

        // STEP 3: Spawn the player.

        _playerSpawner.SpawnPlayer(_configuration.StartCell);

        if (!_playerSpawner.IsPlayerSpawned)
        {
            Debug.LogError("Player spawning failed.", this);
            _gameFlowCoroutine = null;
            yield break;
        }

        // Wait until both the player's entrance and landing effect are complete.
        while (!_playerSpawner.IsEntranceComplete)
        {
            yield return null;
        }

        Debug.Log("Player entrance sequence completed.", this);

        // STEP 4: Spawn the goal.

        _goalSpawner.SpawnGoal(_configuration.GoalCell);

        if (!_goalSpawner.IsGoalSpawned)
        {
            Debug.LogError("Goal spawning failed.", this);
            _gameFlowCoroutine = null;
            yield break;
        }

        Debug.Log("Level preparation completed!", this);

        // Mark the game flow sequence as finished.
        _gameFlowCoroutine = null;
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Flow Controller: GameFlowManager
/// Coordinates the preparation of the level.
/// It ensures that each major stage is executed in the required order.
///
/// The Dependency Layer:
/// LevelGenerator provides the level configuration.
/// MazeGenerator creates the maze.
/// PlayerSpawner creates the player and manages its entrance.
/// GoalSpawner creates the goal.
///
/// The Initialization Pipeline: Start
/// Automatically calls StartGameFlow when the scene starts.
///
/// The Coroutine Controller: StartGameFlow
/// Prevents multiple game flow sequences from running simultaneously
/// and starts the main preparation coroutine.
///
/// The Level Configuration Stage:
/// Calls LevelGenerator.InitializeLevel and checks whether
/// a valid LevelConfiguration is available.
///
/// The Maze Generation Stage:
/// Calls MazeGenerator.GenerateMaze and waits until IsRevealComplete
/// indicates that the maze reveal animation has finished.
///
/// The Player Entrance Stage:
/// Spawns the player at the configured StartCell and waits
/// until the entrance sequence is complete.
///
/// The Goal Creation Stage:
/// Spawns the goal at the configured GoalCell.
///
/// The Completion Stage:
/// Clears the coroutine reference after all preparation stages succeed.
///
/// The Coroutine Pipeline: RunGameFlow
/// Uses yield return null to wait across frames without blocking Unity's
/// main thread. This allows animations and other game systems to continue
/// running while the preparation sequence waits for completion.
/// </summary>
