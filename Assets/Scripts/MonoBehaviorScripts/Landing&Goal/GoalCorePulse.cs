
using System.Collections;
using UnityEngine;

public class GoalCorePulse : MonoBehaviour
{
    [Header("Pulse Settings")]

    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private float pulseAmount = 0.12f;

    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed);

        float scaleMultiplier = 1f + (pulse * pulseAmount);

        transform.localScale = initialScale * scaleMultiplier;
    }
}
