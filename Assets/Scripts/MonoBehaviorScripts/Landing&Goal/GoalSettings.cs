using UnityEngine;

[CreateAssetMenu(
    fileName = "GoalSettings",
    menuName = "EvoLevel/Goal/Goal Settings"
)]
public class GoalSettings : ScriptableObject
{
    [Header("Goal Configuration")]

        [SerializeField] private GameObject _goalPrefab;
    public GameObject GoalPrefab
    {
        get { return _goalPrefab; }
    }
}
