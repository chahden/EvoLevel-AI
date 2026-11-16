
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the player's entrance sequence.
/// Moves the player from an offset starting position to the landing position,
/// plays the landing wave animation, and enables movement when the sequence finishes.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerEntranceController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Settings")]

    [Tooltip("Settings asset containing the player's entrance offsets and animation duration.")]
    [SerializeField] private PlayerEntranceSettings _entranceSettings;

    #endregion

    #region Private Variables

    private Rigidbody2D _playerRigidbody;
    private PlayerMovement _playerMovement;

    // Stores the active entrance coroutine so it can be stopped if another entrance starts.
    private Coroutine _entranceCoroutine;

    #endregion

    #region Initialization

    /// <summary>
    /// Retrieves the required components and prepares the player
    /// before the entrance sequence begins.
    /// </summary>
    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();

        // Disable gravity and rotation to keep the entrance movement controlled.
        _playerRigidbody.gravityScale = 0f;
        _playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Prevent player input from interfering with the entrance animation.
        _playerMovement.SetMovementEnabled(false);
    }

    #endregion

    #region Entrance

    private bool _isEntranceComplete;

    /// <summary>
    /// Indicates whether the complete entrance sequence has finished.
    /// </summary>
    public bool IsEntranceComplete
    {
        get { return _isEntranceComplete; }
    }

    /// <summary>
    /// Positions the player at an offset from the landing position
    /// and starts the entrance animation.
    /// </summary>
    /// <param name="_targetPosition">The final position where the player should land.</param>
    /// <param name="_landingWaveAnimation">The landing wave animation to play after arrival.</param>
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

        // Stop a previous entrance animation before starting a new one.
        if (_entranceCoroutine != null)
        {
            StopCoroutine(_entranceCoroutine);
        }

        // Calculate the starting position using the configured entrance offsets.
        Vector2 _entrancePosition = new Vector2(
            _targetPosition.x + _entranceSettings.EntranceOffsetX,
            _targetPosition.y + _entranceSettings.EntranceOffsetY
        );

        _playerRigidbody.position = _entrancePosition;
        _playerRigidbody.linearVelocity = Vector2.zero;

        // Keep movement disabled throughout the entrance sequence.
        _playerMovement.SetMovementEnabled(false);

        _entranceCoroutine = StartCoroutine(
            AnimateEntrance(_targetPosition, _landingWaveAnimation)
        );
    }

    /// <summary>
    /// Smoothly moves the player from the entrance position
    /// to the target landing position using Rigidbody2D.MovePosition.
    /// </summary>
    /// <param name="_targetPosition">The final landing position.</param>
    /// <param name="_landingWaveAnimation">The landing wave animation to play after movement.</param>
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
            // Use fixedDeltaTime because the movement is performed through Rigidbody2D.
            _elapsedTime += Time.fixedDeltaTime;

            float _progress = Mathf.Clamp01(_elapsedTime / _duration);

            // Smooth the interpolation to avoid a constant-speed, abrupt-looking movement.
            float _smoothProgress = Mathf.SmoothStep(0f, 1f, _progress);

            Vector2 _newPosition = Vector2.Lerp(
                _startingPosition,
                _destination,
                _smoothProgress
            );

            _playerRigidbody.MovePosition(_newPosition);

            // Wait for the next physics update before calculating the next position.
            yield return new WaitForFixedUpdate();
        }

        // Ensure the player reaches the exact destination when the animation ends.
        _playerRigidbody.position = _destination;
        _playerRigidbody.linearVelocity = Vector2.zero;

        _entranceCoroutine = null;

        // Start the landing effect before allowing the player to move.
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

    /// <summary>
    /// Called when the landing wave animation finishes.
    /// Enables player movement and marks the entrance sequence as complete.
    /// </summary>
    private void OnLandingWaveFinished()
    {
        _playerMovement.SetMovementEnabled(true);

        _isEntranceComplete = true;

        Debug.Log("Player entrance completed!", this);
    }

    #endregion
}
