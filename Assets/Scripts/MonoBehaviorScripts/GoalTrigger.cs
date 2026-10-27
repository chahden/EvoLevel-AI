
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null && GameManager.Instance != null)
        {
            GameManager.Instance.WinGame();
        }
    }
}
