
using System;
using System.Collections;
using UnityEngine;

public class LandingWaveAnimation : MonoBehaviour
{
    #region Inspector Fields

    [Header("Animation Settings")]
    [SerializeField] private float _duration = 0.8f;
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
        gameObject.SetActive(false);
    }

    private void InitializeAnimation()
    {
        if (_isInitialized)
        {
            return;
        }

        _initialScale = transform.localScale;

        _waveRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        _initialColors = new Color[_waveRenderers.Length];

        for (int _i = 0; _i < _waveRenderers.Length; _i++)
        {
            if (_waveRenderers[_i] != null)
            {
                _initialColors[_i] = _waveRenderers[_i].color;
            }
        }

        _isInitialized = true;
    }

    #endregion

    #region Animation Control

    public void Play(Action _onComplete = null)
    {
        InitializeAnimation();

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }

        Transform _parent = transform.parent;
        if (_parent != null && !_parent.gameObject.activeSelf)
        {
            _parent.gameObject.SetActive(true);
        }

        enabled = true;
        gameObject.SetActive(true);

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError(
                "LandingWaveAnimation: Cannot play while the parent hierarchy is inactive.",
                this
            );
            return;
        }

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

    private IEnumerator AnimateWave(Action _onComplete)
    {
        float _elapsedTime = 0f;

        Vector3 _targetScale = new Vector3(
            _finalScale,
            _finalScale,
            _initialScale.z
        );

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.deltaTime;

            float _progress = Mathf.Clamp01(_elapsedTime / _duration);

            float _smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                _progress
            );

            transform.localScale = Vector3.Lerp(
                _initialScale,
                _targetScale,
                _smoothProgress
            );

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

            yield return null;
        }

        transform.localScale = _targetScale;

        _animationCoroutine = null;

        gameObject.SetActive(false);

        _onComplete?.Invoke();
    }

    #endregion
}
