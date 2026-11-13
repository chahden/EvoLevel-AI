
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _landingPlatformPrefab;

    #endregion

    #region Private Variables

    private GameObject _playerInstance;
    private GameObject _landingPlatformInstance;

    private PlayerEntranceController _entranceController;

    #endregion
    public bool IsPlayerSpawned
{
    get { return _playerInstance != null; }
}
        public bool IsEntranceComplete
    {
        get
        {
            if (_entranceController == null)
            {
                return false;
            }

            return _entranceController.IsEntranceComplete;
        }
    }

    #region Player Spawning

    public void SpawnPlayer(Vector2Int _startCell)
    {
        if (_playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: Player prefab is missing!", this);
            return;
        }

        if (_gridManager == null)
        {
            Debug.LogError("PlayerSpawner: GridManager reference is missing!", this);
            return;
        }

        if (!_gridManager.IsInsideGrid(_startCell))
        {
            Debug.LogError("PlayerSpawner: Start cell is outside the grid!", this);
            return;
        }

        if (_playerInstance != null)
        {
            Debug.LogWarning("PlayerSpawner: Player already exists!", this);
            return;
        }

        Vector2 _startPosition = _gridManager.GridToWorld(_startCell);

        Vector3 _spawnPosition = new Vector3(
            _startPosition.x,
            _startPosition.y,
            transform.position.z
        );

        CreateLandingPlatform(_spawnPosition);

        _playerInstance = Instantiate(
            _playerPrefab,
            _spawnPosition,
            transform.rotation
        );

        _entranceController =
            _playerInstance.GetComponent<PlayerEntranceController>();

        if (_entranceController == null)
        {
            Debug.LogError(
                "PlayerSpawner: PlayerEntranceController is missing from the player prefab!",
                _playerInstance
            );

            Destroy(_playerInstance);
            _playerInstance = null;
            return;
        }

        _entranceController.PlayEntrance(
            _spawnPosition,
            GetLandingWaveAnimation()
        );
    }

    public void RespawnPlayer(Vector2Int _startCell)
    {
        ClearPlayer();
        SpawnPlayer(_startCell);
    }

    public void ClearPlayer()
    {
        if (_playerInstance != null)
        {
            Destroy(_playerInstance);
            _playerInstance = null;
        }

        if (_landingPlatformInstance != null)
        {
            Destroy(_landingPlatformInstance);
            _landingPlatformInstance = null;
        }

        _entranceController = null;
    }

    #endregion

    #region Landing Platform

    private void CreateLandingPlatform(Vector3 _position)
    {
        if (_landingPlatformPrefab == null)
        {
            Debug.LogWarning(
                "PlayerSpawner: Landing platform prefab is missing!",
                this
            );

            return;
        }

        _landingPlatformInstance = Instantiate(
            _landingPlatformPrefab,
            _position,
            Quaternion.identity
        );

        _landingPlatformInstance.name = "LandingPlatform";
        _landingPlatformInstance.SetActive(true);
    }

    private LandingWaveAnimation GetLandingWaveAnimation()
    {
        if (_landingPlatformInstance == null)
        {
            return null;
        }

        return _landingPlatformInstance
            .GetComponentInChildren<LandingWaveAnimation>(true);
    }

    #endregion
}
