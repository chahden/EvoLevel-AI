using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    #region Inspector Fields
        [Header("Refrence")]
            [Tooltip("Reference to the GridManager that handles grid boundaries and coordinate conversions.")]
                public GridManager gridManager;
            [Tooltip("Prefab instantiated as an obstacle on the grid.")]
                public GameObject obstaclePrefab;

        [Header("Settings")]
            [Tooltip("Percentage chance (from 0 to 1) of a grid cell containing an obstacle.")]
                [Range(0f, 1f)]
                public float obstacleDensity = 0.2f;
    #endregion

    #region Unity Lifecycle
    void Start()
    {
        GenerateLevel();

    }
    #endregion

    #region Level Generation Methods
    /// <summary>
    /// Iterates through the grid dimensions and randomly spawns obstacles based on the configured obstacle density.
    /// </summary>
        void GenerateLevel()
        {
            if (gridManager == null || obstaclePrefab == null)
            {
                Debug.LogError("Missing GridManager or Obstacle Prefab!");
                return;
            }

            for (int x = 0; x < gridManager._width; x++)
            {
                for (int y = 0; y < gridManager._height; y++)
                {
                    float randomValue = Random.value;

                    if (randomValue < obstacleDensity)
                    {
                        Vector2Int gridPosition = new Vector2Int(x, y);

                        Vector2 worldPosition =
                            gridManager.WorldToGrid(gridPosition);

                        Instantiate(
                            obstaclePrefab,
                            worldPosition,
                            Quaternion.identity,
                            transform
                        );
                    }
                }
            }
        }
    #endregion
}

 

