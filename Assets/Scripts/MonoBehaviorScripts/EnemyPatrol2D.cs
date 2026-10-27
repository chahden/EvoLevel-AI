
using UnityEngine;

public class EnemyAutoPatrol2D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private EnemyPatrolSettings settings;
    [SerializeField] private bool _patrolEnabled = true;

    private Vector2 _areaCenter;
    private Vector2 _target;
    private bool _hasTarget;
    private float _waitTimer;

    private void Start()
    {
        _areaCenter = transform.position;
        ChooseDestination();
    }

    private void Update()
    {
        if (!_patrolEnabled || settings == null)return;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime; return;
        }

        if (!_hasTarget)
        {
            ChooseDestination();return;
        }

        Vector2 position = transform.position;
        Vector2 direction = (_target - position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            position,
            direction,
            settings._detectionDistance,
            settings._ObstacleLayer
        );

        if (hit.collider != null)
        {
            _hasTarget = false;
            _waitTimer = settings._waitBeforNewTarget;return;
        }

        transform.position = Vector2.MoveTowards(
            position,
            _target,
            settings._speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, _target)
            <= settings._arricalDistance)
        {
            ChooseDestination();
        }
    }

    private void ChooseDestination()
    {
        if (settings == null)return;

        float halfWidth = settings._areaSize.x / 2f;
        float halfHeight = settings._areaSize.y / 2f;

        _target = new Vector2(
            Random.Range(
                _areaCenter.x - halfWidth,
                _areaCenter.x + halfWidth
            ),
            Random.Range(
                _areaCenter.y - halfHeight,
                _areaCenter.y + halfHeight
            )
        );

        _hasTarget = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (settings == null)return;

        Vector2 center = Application.isPlaying
            ? _areaCenter
            : (Vector2)transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, settings._areaSize);

        if (Application.isPlaying && _hasTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_target, 0.15f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _target);
        }
    }
}
