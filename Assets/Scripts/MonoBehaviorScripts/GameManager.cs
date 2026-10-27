using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool _gameEnded;
    private float _startTime;
    public bool GameEnded
    {
        get { return _gameEnded; }
    } 

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);return;
        }
        Instance = this;
    }
    public void Start()
    {
        _startTime = Time.time;
        Debug.Log("Game started! Timer is running.");
    }
    public void WinGame()
    {
        if (_gameEnded)return;
        _gameEnded = true;

        float timeSpent = Time.time - _startTime;

        Debug.Log("You Win!");
        Debug.Log($"Time to reach the goal: {timeSpent:F2} seconds");
    }

    public void LoseGame()
    {
        if (_gameEnded)return;
        _gameEnded = true;
        Debug.Log("You Lose!");
    }
    public void RestartGameState()
    {
        _gameEnded = false;
        _startTime = Time.time;

        Debug.Log("Game restarted! Timer reset.");
    }

}
