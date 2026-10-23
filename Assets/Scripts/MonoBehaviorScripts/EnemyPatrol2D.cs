
using UnityEngine;

public class EnemyAutoPatrol2D : MonoBehaviour
{
    [Header("Patrol")]
    public bool patrolEnabled = true;
    public float speed = 2f;

    [Header("Patrol Area")]
    public Vector2 areaSize = new Vector2(8f, 8f);
    public Vector2 areaCenter;

    [Header("Destination")]
    public float arrivalDistance = 0.15f;

    [Header("Obstacle Detection")]
    public LayerMask obstacleLayer;
    public float detectionDistance = 0.5f;
    public float waitBeforeNewTarget = 0.5f;

    private Vector2 target;
    private bool hasTarget;
    private float waitTimer;

    void Start()
    {
        areaCenter = transform.position;
        ChooseDestination();
    }

    void Update()
    {
        if (!patrolEnabled)
        {
            return;
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        if (!hasTarget)
        {
            ChooseDestination();
            return;
        }

        Vector2 direction =
            (target - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            detectionDistance,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            hasTarget = false;
            waitTimer = waitBeforeNewTarget;
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target)
            < arrivalDistance)
        {
            ChooseDestination();
        }
    }

    void ChooseDestination()
    {
        float halfWidth = areaSize.x / 2f;
        float halfHeight = areaSize.y / 2f;

        target = new Vector2(
            Random.Range(
                areaCenter.x - halfWidth,
                areaCenter.x + halfWidth
            ),
            Random.Range(
                areaCenter.y - halfHeight,
                areaCenter.y + halfHeight
            )
        );

        hasTarget = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            new Vector3(areaCenter.x, areaCenter.y, 0f),
            new Vector3(areaSize.x, areaSize.y, 0f)
        );

        if (hasTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(target, 0.15f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target);
        }
    }
}
