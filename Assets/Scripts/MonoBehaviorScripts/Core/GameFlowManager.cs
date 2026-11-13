
using System.Collections;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private MazeGenerator _mazeGenerator;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private GoalSpawner _goalSpawner;

    private Coroutine _gameFlowCoroutine;

    private void Start()
    {
        StartGameFlow();
    }

    public void StartGameFlow()
    {
        if (_gameFlowCoroutine != null)
        {
            Debug.LogWarning("Game flow is already running.", this);
            return;
        }

        _gameFlowCoroutine = StartCoroutine(RunGameFlow());
    }

   
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

    _gameFlowCoroutine = null;
}

}
