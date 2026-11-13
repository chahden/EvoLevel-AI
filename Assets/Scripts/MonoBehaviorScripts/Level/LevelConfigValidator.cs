
using UnityEngine;

public static class LevelConfigValidator
{
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
}
