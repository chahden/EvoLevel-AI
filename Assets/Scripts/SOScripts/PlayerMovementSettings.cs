using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementSettings", menuName = "Scriptable Objects/PlayerMovementSettings")]
public class PlayerMovementSettings : ScriptableObject
{
    [Header("Player Movement Settings")]
    [Min(0f)]
    public float _speed = 5f;
}
