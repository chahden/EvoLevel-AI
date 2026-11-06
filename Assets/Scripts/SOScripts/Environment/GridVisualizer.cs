
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    #region Inspector Fields
        [Header("Refrence")]
            [Tooltip("Reference to the GridManager script that defines grid dimensions and origin.")]
                public GridManager gridManager;
            [Tooltip("Prefab used to visualize individual grid cells.")]
                public GameObject gridCellPrefab;
    #endregion

    #region Unity Lifecycle
        void Start()
        {
            GenerateGrid();
        }
    #endregion

    #region Grid Generation Methods
    /// <summary>
    /// Generates and instantiates the visual grid cells based on GridManager settings.
    /// </summary>
        public void GenerateGrid()
        {
            if (gridManager == null || gridCellPrefab == null)
            {
                Debug.LogError("GridManager or Cell Prefab is missing!");
                return;
            }

            for (int x = 0 ; x < gridManager._width; x++)
            {
                for (int y = 0 ; y  < gridManager._height; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);

                    Vector2 worldPosition = gridManager.WorldToGrid(gridPosition);

                    GameObject cell = Instantiate(
                        gridCellPrefab,
                        worldPosition,
                        Quaternion.identity,
                        transform
                    );
                    cell.name = $"Cell {x}, {y}";
                }

            }

         }
    #endregion

}
