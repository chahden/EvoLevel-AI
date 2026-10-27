using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPatrolSettings", menuName = "Scriptable Objects/EnemyPatrolSettings")]
public class EnemyPatrolSettings : ScriptableObject
{
    [Header("Mouvement")]
    [Min(0f)]
    public float _speed = 2f;

    [Header("Patrol Area")]
    public Vector2 _areaSize =  new Vector2(8f, 8f);

    [Header("Destination")]
    [Min(0f)]
    public float _arricalDistance = 0.15f;

    [Header("Obstacle Detection")]
    public LayerMask _ObstacleLayer;
    [Min(0f)]
    public float _detectionDistance;
    [Min(0f)]
    public float _waitBeforNewTarget;

}
