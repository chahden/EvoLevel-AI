
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerEntranceController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Settings")]

    [SerializeField] private PlayerEntranceSettings _entranceSettings;

    #endregion

    #region Private Variables

    private Rigidbody2D _playerRigidbody;
    private PlayerMovement _playerMovement;

    private Coroutine _entranceCoroutine;

    #endregion

    #region Initialization

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();

        _playerRigidbody.gravityScale = 0f;
        _playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        _playerMovement.SetMovementEnabled(false);
    }

    #endregion

    #region Entrance
        private bool _isEntranceComplete;

        public bool IsEntranceComplete
        {
            get { return _isEntranceComplete; }
        }
    public void PlayEntrance(
        Vector3 _targetPosition,
        LandingWaveAnimation _landingWaveAnimation)
    {
        if (_entranceSettings == null)
        {
            Debug.LogError(
                "PlayerEntranceController: Entrance settings are missing!",
                this
            );
            return;
        }

        if (_entranceCoroutine != null)
        {
            StopCoroutine(_entranceCoroutine);
        }

        Vector2 _entrancePosition = new Vector2(
            _targetPosition.x + _entranceSettings.EntranceOffsetX,
            _targetPosition.y + _entranceSettings.EntranceOffsetY
        );

        _playerRigidbody.position = _entrancePosition;
        _playerRigidbody.linearVelocity = Vector2.zero;

        _playerMovement.SetMovementEnabled(false);

        _entranceCoroutine = StartCoroutine(
            AnimateEntrance(_targetPosition, _landingWaveAnimation)
        );
    }

    private IEnumerator AnimateEntrance(
        Vector3 _targetPosition,
        LandingWaveAnimation _landingWaveAnimation)
    {
        Vector2 _startingPosition = _playerRigidbody.position;
        Vector2 _destination = _targetPosition;

        float _elapsedTime = 0f;
        float _duration = _entranceSettings.EntranceDuration;

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.fixedDeltaTime;

            float _progress = Mathf.Clamp01(_elapsedTime / _duration);
            float _smoothProgress = Mathf.SmoothStep(0f, 1f, _progress);

            Vector2 _newPosition = Vector2.Lerp(
                _startingPosition,
                _destination,
                _smoothProgress
            );

            _playerRigidbody.MovePosition(_newPosition);

            yield return new WaitForFixedUpdate();
        }

        _playerRigidbody.position = _destination;
        _playerRigidbody.linearVelocity = Vector2.zero;

        _entranceCoroutine = null;

        if (_landingWaveAnimation != null)
        {
            _landingWaveAnimation.Play(OnLandingWaveFinished);
        }
        else
        {
            OnLandingWaveFinished();
        }
    }

    #endregion

    #region Completion

    private void OnLandingWaveFinished()
    {
        _playerMovement.SetMovementEnabled(true);
        _isEntranceComplete = true;

        Debug.Log("Player entrance completed!", this);
    }

    #endregion
}
