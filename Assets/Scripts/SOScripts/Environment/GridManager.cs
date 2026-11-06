using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Inspector Fields
        [Header("Grid Settings")]

            [Tooltip("Number of columns in the grid.")]
                public int _width = 10;
            [Tooltip("Number of rows in the grid.")]
                public int _height = 10;
            [Tooltip("Size of each individual cell in world units.")]
                public float _cellSize = 1.0f;


        [Header("Grid origins")]

            [Tooltip("World position of the bottom-left corner of the grid.")]
                public Vector2 _originePosition;
    #endregion


    #region Grid Conversion Methods
    
    /// <summary>
    /// Converts a world position (Vector2) to grid coordinates (Vector2Int).
    /// </summary>
   
        public Vector2Int WorldToGrid(Vector2 worldPosition)
        {
                int x = Mathf.FloorToInt(
                    (worldPosition.x - _originePosition.x) / _cellSize
                );

                int y = Mathf.FloorToInt(
                    (worldPosition.y - _originePosition.y) / _cellSize
                );

                return new Vector2Int(x, y);
   }
   #endregion

    #region Validation Methods

    /// <summary>
    /// Checks whether a given world position falls within the grid boundaries.
    /// </summary>
    
        public bool IsInsideGrid(Vector2 worldPosition)
            {
                Vector2Int gridPosition = WorldToGrid(worldPosition);
                if(gridPosition.x < 0 || gridPosition.x >= _width) return false;

                if(gridPosition.y < 0 || gridPosition.y >= _height) return false;
                return true;
            }
    #endregion
    
}

