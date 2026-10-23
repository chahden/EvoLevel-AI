
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    private GameObject playerInstance;

    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        playerInstance = Instantiate(
            playerPrefab,
            transform.position,
            transform.rotation
        );
    }

    public void RespawnPlayer()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }

        SpawnPlayer();
    }
}
