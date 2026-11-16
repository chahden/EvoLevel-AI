using UnityEngine;


/// <summary>
/// Validates runtime level configurations by checking grid dimensions,
/// cell size, starting position, and goal position.
/// </summary>

public static class LevelConfigValidator
{
    #region Configuration Validation


    /// <summary>
    /// Checks whether the provided level configuration is valid.
    /// Returns false and provides an error message if a validation
    /// condition is not satisfied.
    /// </summary>
    public static bool Validate(
        LevelConfiguration configuration,
        out string errorMessage
    )
        {
            errorMessage = "";

            // Check that a configuration exists.
            if (configuration == null)
            {
                errorMessage = "Level configuration is missing!";
                return false;
            }

            // Check grid dimensions.
            if (configuration.Width < 1 || configuration.Height < 1)
            {
                errorMessage = "Grid width and height must be at least 1!";
                return false;
            }

            // Check cell size.
            if (configuration.CellSize <= 0f)
            {
                errorMessage = "Cell size must be greater than 0!";
                return false;
            }

            // Check start position.
            if (!IsInsideGrid(
                configuration.StartCell,
                configuration.Width,
                configuration.Height
            ))
            {
                errorMessage = "Start position is outside the grid!";
                return false;
            }

            // Check goal position.
            if (!IsInsideGrid(
                configuration.GoalCell,
                configuration.Width,
                configuration.Height
            ))
            {
                errorMessage = "Goal position is outside the grid!";
                return false;
            }

            // Check that start and goal are different.
            if (configuration.StartCell == configuration.GoalCell)
            {
                errorMessage = "Start and goal cannot be the same cell!";
                return false;
            }

            return true;
        }

    #endregion


    #region Position Validation


    /// <summary>
    /// Checks whether a cell position is within the grid boundaries
    /// using the provided grid dimensions.
    /// </summary>
    private static bool IsInsideGrid(
        Vector2Int _position,
        int _width,
        int _height
    )
    {
        if (_position.x < 0 || _position.x >= _width)
        {
            return false;
        }

        if (_position.y < 0 || _position.y >= _height)
        {
            return false;
        }

        return true;
    }
    #endregion



/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Validation Hub: LevelConfigValidator
/// Provides centralized validation for runtime level configurations
/// before they are used by other gameplay systems.
///
/// The Configuration Validation Pipeline: Validate
/// Checks that a configuration exists, grid dimensions are valid,
/// cell size is positive, and starting and goal positions
/// satisfy the level requirements.
///
/// The Position Validation Pipeline: IsInsideGrid
/// Checks whether a given cell position is within the grid boundaries
/// using the configured width and height.
///
/// The Error Reporting System: Error Message
/// Returns a descriptive error message through the out parameter
/// when a validation condition fails.
///
/// The System Integration:
/// LevelGenerator uses this validator before initializing GridManager.
/// GridManager also validates configurations when receiving them,
/// providing an additional validation point at its initialization boundary.
/// </summary>
}
