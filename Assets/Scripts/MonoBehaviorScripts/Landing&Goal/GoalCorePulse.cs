
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the pulsing animation of the goal core
/// by continuously changing its local scale.
/// </summary>

public class GoalCorePulse : MonoBehaviour
{
    #region Inspector Fields

    [Header("Pulse Settings")]
    
    [Tooltip("Controls the speed of the pulsing animation.")]
    [SerializeField] private float pulseSpeed = 1.5f;

    [Tooltip("Controls the intensity of the scale variation.")]
    [SerializeField] private float pulseAmount = 0.12f;
    #endregion
    
    #region Private Variables
    private Vector3 initialScale;

    #endregion

    #region Initialization

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    #endregion
    
    #region Animation
    private void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed);

        float scaleMultiplier = 1f + (pulse * pulseAmount);

        transform.localScale = initialScale * scaleMultiplier;
    }
    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Animation Controller: GoalCorePulse
/// Controls the visual pulsing effect of the goal core
/// by continuously modifying its local scale.
///
/// The Initialization Pipeline: Awake
/// Stores the original scale of the object before the animation begins.
/// This scale is used as the reference for all subsequent changes.
///
/// The Animation Pipeline: Update
/// Uses a sine wave to generate a smooth oscillating value.
/// This value is multiplied by the pulse amount to calculate
/// the scale variation, which is then applied to the original scale.
///
/// The Animation Parameters: Pulse Settings
/// Pulse Speed controls how quickly the core expands and contracts.
/// Pulse Amount controls how much the core changes in size.
/// </summary>
