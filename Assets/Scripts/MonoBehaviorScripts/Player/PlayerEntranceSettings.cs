
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerEntranceSettings",
    menuName = "EvoLevel/Player/Entrance Settings"
)]
public class PlayerEntranceSettings : ScriptableObject
{
    #region Entrance Settings

    [Header("Entrance Offset")]

    [SerializeField] private float _entranceOffsetX = 3f;

    [SerializeField] private float _entranceOffsetY = 3f;


    [Header("Animation")]

    [Min(0.1f)]
    [SerializeField] private float _entranceDuration = 1.2f;

    #endregion


    #region Properties

    public float EntranceOffsetX
    {
        get { return _entranceOffsetX; }
    }

    public float EntranceOffsetY
    {
        get { return _entranceOffsetY; }
    }

    public float EntranceDuration
    {
        get { return _entranceDuration; }
    }

    #endregion
}
