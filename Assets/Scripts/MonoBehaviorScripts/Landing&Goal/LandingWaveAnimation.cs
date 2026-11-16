
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the landing wave animation by expanding its scale
/// and gradually fading its SpriteRenderers.
/// </summary>
public class LandingWaveAnimation : MonoBehaviour
{
    #region Inspector Fields

    [Header("Animation Settings")]

    [Tooltip("Duration of the landing wave animation in seconds.")]
    [SerializeField] private float _duration = 0.8f;

    [Tooltip("Final scale reached by the landing wave animation.")]
    [SerializeField] private float _finalScale = 1.5f;

    #endregion

    #region Private Variables

    private Vector3 _initialScale;
    private SpriteRenderer[] _waveRenderers;
    private Color[] _initialColors;

    private Coroutine _animationCoroutine;

    private bool _isInitialized;

    #endregion

    #region Initialization

    private void Awake()
    {
        InitializeAnimation();

        // Keep the animation object inactive until it is requested.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Caches the initial scale and colors required by the animation.
    /// </summary>
    private void InitializeAnimation()
    {
        if (_isInitialized)
        {
            return;
        }

        _initialScale = transform.localScale;

        // Include inactive child objects when collecting the wave renderers.
        _waveRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        _initialColors = new Color[_waveRenderers.Length];

        for (int _i = 0; _i < _waveRenderers.Length; _i++)
        {
            if (_waveRenderers[_i] != null)
            {
                // Preserve each renderer's original color and alpha.
                _initialColors[_i] = _waveRenderers[_i].color;
            }
        }

        _isInitialized = true;
    }

    #endregion

    #region Animation Control

    /// <summary>
    /// Starts the landing wave animation and invokes an optional callback
    /// after the animation completes.
    /// </summary>
    public void Play(Action _onComplete = null)
    {
        InitializeAnimation();

        // Stop any previous animation before starting a new one.
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }

        // Ensure the parent is active so the animation can run.
        Transform _parent = transform.parent;

        if (_parent != null && !_parent.gameObject.activeSelf)
        {
            _parent.gameObject.SetActive(true);
        }

        enabled = true;
        gameObject.SetActive(true);

        // Verify that the animation is active in the complete hierarchy.
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError(
                "LandingWaveAnimation: Cannot play while the parent hierarchy is inactive.",
                this
            );

            return;
        }

        // Reset the animation to its original visual state.
        transform.localScale = _initialScale;

        for (int _i = 0; _i < _waveRenderers.Length; _i++)
        {
            if (_waveRenderers[_i] != null)
            {
                _waveRenderers[_i].color = _initialColors[_i];
            }
        }

        _animationCoroutine = StartCoroutine(AnimateWave(_onComplete));
    }

    #endregion

    #region Animation

    /// <summary>
    /// Expands the landing wave and fades its SpriteRenderers
    /// over the configured animation duration.
    /// </summary>
    private IEnumerator AnimateWave(Action _onComplete)
    {
        float _elapsedTime = 0f;

        // Preserve the original Z scale while changing X and Y.
        Vector3 _targetScale = new Vector3(
            _finalScale,
            _finalScale,
            _initialScale.z
        );

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.deltaTime;

            // Calculate normalized animation progress between 0 and 1.
            float _progress = Mathf.Clamp01(_elapsedTime / _duration);

            // Smooth the progress to avoid a linear, abrupt animation.
            float _smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                _progress
            );

            // Gradually expand the wave from its initial scale.
            transform.localScale = Vector3.Lerp(
                _initialScale,
                _targetScale,
                _smoothProgress
            );

            // Gradually reduce the alpha of each wave renderer.
            for (int _i = 0; _i < _waveRenderers.Length; _i++)
            {
                if (_waveRenderers[_i] == null)
                {
                    continue;
                }

                Color _fadedColor = _initialColors[_i];

                _fadedColor.a = Mathf.Lerp(
                    _initialColors[_i].a,
                    0f,
                    _smoothProgress
                );

                _waveRenderers[_i].color = _fadedColor;
            }

            // Wait for the next frame before updating the animation.
            yield return null;
        }

        // Ensure the animation reaches its exact final state.
        transform.localScale = _targetScale;

        _animationCoroutine = null;

        // Hide the wave after the animation has completed.
        gameObject.SetActive(false);

        // Notify the caller that the animation has finished.
        _onComplete?.Invoke();
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Animation Controller: LandingWaveAnimation
/// Manages the landing wave visual effect by expanding its scale
/// and gradually fading its SpriteRenderers.
///
/// The Initialization Pipeline: Awake and InitializeAnimation
/// Stores the original scale and the initial colors of all child
/// SpriteRenderers. These values are used to restore the animation
/// to its original state whenever it is played.
///
/// The Animation Control Pipeline: Play
/// Stops any previous animation, ensures the required hierarchy
/// is active, restores the initial visual state, and starts the
/// animation coroutine.
///
/// The Animation Pipeline: AnimateWave
/// Uses elapsed time to calculate the animation progress.
/// SmoothStep creates a gradual transition, while Lerp interpolates
/// the scale and alpha values toward their final states.
///
/// The Coroutine Management: _animationCoroutine
/// Keeps track of the active animation so it can be stopped
/// before another animation starts.
///
/// The Completion Callback: _onComplete
/// Invokes an optional callback after the animation finishes,
/// allowing another system to continue its sequence.
///
/// The Visibility Control:
/// Disables the animation GameObject after completion so the
/// landing wave remains hidden until it is played again.
/// </summary>
