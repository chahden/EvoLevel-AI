
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Generates the maze structure using an iterative Depth-First Search algorithm,
/// manages wall creation, and controls the progressive maze reveal animation.
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]

    [Tooltip("Grid manager providing the grid dimensions and coordinate conversions.")]
    [FormerlySerializedAs("gridManager")]
    [SerializeField] private GridManager _gridManager;

    [Header("Wall Settings")]

    [Tooltip("Prefab used to create individual maze walls.")]
    [FormerlySerializedAs("wallPrefab")]
    [SerializeField] private GameObject _wallPrefab;

    [Tooltip("Parent transform used to organize all generated maze walls.")]
    [FormerlySerializedAs("wallParent")]
    [SerializeField] private Transform _wallParent;

    [Tooltip("Thickness of the generated maze walls.")]
    [FormerlySerializedAs("wallThickness")]
    [SerializeField] private float _wallThickness = 0.12f;

    [Header("Maze Reveal Animation")]

    [Tooltip("Total duration of the maze reveal animation in seconds.")]
    [FormerlySerializedAs("revealDuration")]
    [Min(0.1f)]
    [SerializeField] private float _revealDuration = 1.2f;

    [Tooltip("Width of the expanding wave used to reveal the maze walls.")]
    [FormerlySerializedAs("revealWaveWidth")]
    [Min(0.1f)]
    [SerializeField] private float _revealWaveWidth = 2f;

    #endregion

    #region Cell Structure

    /// <summary>
    /// Represents an individual maze cell.
    /// Stores the state of its four walls and whether it has been visited.
    /// </summary>
    [System.Serializable]
    public class Cell
    {
        public bool top = true;
        public bool bottom = true;
        public bool left = true;
        public bool right = true;

        public bool visited = false;
    }

    private Cell[,] _maze;

    /// <summary>
    /// Stores the position, length, and rotation of a wall
    /// before its GameObject is created.
    /// </summary>
    private class WallData
    {
        public Vector2 Position;
        public float Length;
        public float Rotation;

        public WallData(Vector2 _position, float _length, float _rotation)
        {
            Position = _position;
            Length = _length;
            Rotation = _rotation;
        }
    }

    /// <summary>
    /// Stores the visual information required to reveal a wall,
    /// including its distance from the starting position
    /// and the original colors of its SpriteRenderers.
    /// </summary>
    private class WallRevealData
    {
        public float Distance;
        public SpriteRenderer[] Renderers;
        public Color[] OriginalColors;

        public WallRevealData(float _distance, SpriteRenderer[] _renderers)
        {
            Distance = _distance;
            Renderers = _renderers;

            OriginalColors = new Color[Renderers.Length];

            for (int _i = 0; _i < Renderers.Length; _i++)
            {
                OriginalColors[_i] = Renderers[_i].color;

                // Hide the wall initially by setting its alpha to zero.
                Color _hiddenColor = OriginalColors[_i];
                _hiddenColor.a = 0f;

                Renderers[_i].color = _hiddenColor;
            }
        }
    }

    private Coroutine _wallCreationCoroutine;
    private bool _isRevealComplete;

    #region Properties

    /// <summary>
    /// Indicates whether the maze reveal animation has completed.
    /// </summary>
    public bool IsRevealComplete
    {
        get { return _isRevealComplete; }
    }

    /// <summary>
    /// Indicates whether maze data has been generated.
    /// </summary>
    public bool IsGenerated
    {
        get { return _maze != null; }
    }

    #endregion

    #endregion

    #region Maze Generation

    /// <summary>
    /// Generates the maze data and creates its visual walls.
    /// In Play Mode, the walls are revealed progressively.
    /// Outside Play Mode, the walls are created immediately.
    /// </summary>
    [ContextMenu("Generate Maze")]
    public void GenerateMaze()
    {
        if (!ValidateReferences())
        {
            return;
        }

        // The reveal is not complete while a new maze is being generated.
        _isRevealComplete = false;

        // Stop an existing reveal before generating another maze.
        if (_wallCreationCoroutine != null)
        {
            StopCoroutine(_wallCreationCoroutine);
            _wallCreationCoroutine = null;
        }

        InitializeMaze();

        GenerateDFS();

        PrepareWallParent();

        if (Application.isPlaying)
        {
            ClearExistingWalls();

            _wallCreationCoroutine = StartCoroutine(
                AnimateMazeGeneration()
            );
        }
        else
        {
            CreateMazeWallsImmediately();
            _isRevealComplete = true;
        }

        Debug.Log("Maze data generated successfully!", this);
    }

    /// <summary>
    /// Checks whether the references and configuration
    /// required for maze generation are valid.
    /// </summary>
    private bool ValidateReferences()
    {
        if (_gridManager == null)
        {
            Debug.LogError(
                "MazeGenerator: GridManager reference is missing!",
                this
            );

            return false;
        }

        if (!_gridManager.IsInitialized)
        {
            Debug.LogError(
                "MazeGenerator: GridManager has not been initialized. "
                + "Initialize the level configuration first!",
                this
            );

            return false;
        }

        if (_wallPrefab == null)
        {
            Debug.LogError(
                "MazeGenerator: Wall Prefab is missing!",
                this
            );

            return false;
        }

        if (_gridManager.Width <= 0 || _gridManager.Height <= 0)
        {
            Debug.LogError(
                "MazeGenerator: Invalid grid dimensions!",
                this
            );

            return false;
        }

        if (_gridManager.CellSize <= 0f)
        {
            Debug.LogError(
                "MazeGenerator: Invalid cell size!",
                this
            );

            return false;
        }

        if (_wallThickness <= 0f)
        {
            Debug.LogError(
                "MazeGenerator: Wall thickness must be greater than zero!",
                this
            );

            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates and initializes the maze array
    /// using the dimensions provided by GridManager.
    /// </summary>
    private void InitializeMaze()
    {
        _maze = new Cell[
            _gridManager.Width,
            _gridManager.Height
        ];

        for (int _x = 0; _x < _gridManager.Width; _x++)
        {
            for (int _y = 0; _y < _gridManager.Height; _y++)
            {
                // Every cell starts with all four walls intact.
                _maze[_x, _y] = new Cell();
            }
        }
    }

    #endregion

    #region Depth First Search

    /// <summary>
    /// Generates the maze using iterative Depth-First Search.
    /// A stack is used to explore unvisited neighboring cells
    /// and backtrack when no unvisited neighbors remain.
    /// </summary>
    private void GenerateDFS()
    {
        Stack<Vector2Int> _stack = new Stack<Vector2Int>();

        Vector2Int _currentCell = Vector2Int.zero;

        _maze[_currentCell.x, _currentCell.y].visited = true;

        _stack.Push(_currentCell);

        while (_stack.Count > 0)
        {
            // Peek returns the current cell without removing it from the stack.
            _currentCell = _stack.Peek();

            List<Vector2Int> _neighbors =
                GetUnvisitedNeighbors(_currentCell);

            if (_neighbors.Count > 0)
            {
                // Randomly select one available neighboring cell.
                int _randomIndex = Random.Range(0, _neighbors.Count);

                Vector2Int _nextCell = _neighbors[_randomIndex];

                // Remove the shared wall to connect the two cells.
                RemoveWall(_currentCell, _nextCell);

                _maze[_nextCell.x, _nextCell.y].visited = true;

                // Push the newly visited cell to continue exploring.
                _stack.Push(_nextCell);
            }
            else
            {
                // Backtrack when the current cell has no unvisited neighbors.
                _stack.Pop();
            }
        }
    }

    /// <summary>
    /// Retrieves all neighboring cells that are inside the grid
    /// and have not yet been visited.
    /// </summary>
    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int _cellPosition)
    {
        List<Vector2Int> _neighbors = new List<Vector2Int>();

        Vector2Int[] _directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int _direction in _directions)
        {
            Vector2Int _neighbor = _cellPosition + _direction;

            // Ignore neighbors outside the grid boundaries.
            if (!_gridManager.IsInsideGrid(_neighbor))
            {
                continue;
            }

            // Only unvisited neighbors can be explored.
            if (!_maze[_neighbor.x, _neighbor.y].visited)
            {
                _neighbors.Add(_neighbor);
            }
        }

        return _neighbors;
    }

    /// <summary>
    /// Removes the shared wall between two adjacent cells
    /// to create a passage between them.
    /// </summary>
    private void RemoveWall(Vector2Int _current, Vector2Int _next)
    {
        Vector2Int _direction = _next - _current;

        // Update both cells to keep their shared wall states synchronized.
        if (_direction == Vector2Int.up)
        {
            _maze[_current.x, _current.y].top = false;
            _maze[_next.x, _next.y].bottom = false;
        }
        else if (_direction == Vector2Int.down)
        {
            _maze[_current.x, _current.y].bottom = false;
            _maze[_next.x, _next.y].top = false;
        }
        else if (_direction == Vector2Int.left)
        {
            _maze[_current.x, _current.y].left = false;
            _maze[_next.x, _next.y].right = false;
        }
        else if (_direction == Vector2Int.right)
        {
            _maze[_current.x, _current.y].right = false;
            _maze[_next.x, _next.y].left = false;
        }
    }

    #endregion

    #region Movement Validation

    /// <summary>
    /// Checks whether movement from the current cell
    /// in the specified direction is possible.
    /// </summary>
    public bool CanMove(Vector2Int _currentCell, Vector2Int _direction)
    {
        if (_maze == null || _gridManager == null)
        {
            return false;
        }

        if (!_gridManager.IsInitialized)
        {
            return false;
        }

        // Ensure the generated maze dimensions match the current grid.
        if (_maze.GetLength(0) != _gridManager.Width ||
            _maze.GetLength(1) != _gridManager.Height)
        {
            return false;
        }

        if (!_gridManager.IsInsideGrid(_currentCell))
        {
            return false;
        }

        // Only the four cardinal directions are supported.
        bool _isValidDirection =
            _direction == Vector2Int.up ||
            _direction == Vector2Int.down ||
            _direction == Vector2Int.left ||
            _direction == Vector2Int.right;

        if (!_isValidDirection)
        {
            return false;
        }

        Vector2Int _nextCell = _currentCell + _direction;

        // Movement cannot lead outside the grid.
        if (!_gridManager.IsInsideGrid(_nextCell))
        {
            return false;
        }

        Cell _cell = _maze[_currentCell.x, _currentCell.y];

        // A movement is allowed when the corresponding wall is absent.
        if (_direction == Vector2Int.up)
        {
            return !_cell.top;
        }

        if (_direction == Vector2Int.down)
        {
            return !_cell.bottom;
        }

        if (_direction == Vector2Int.left)
        {
            return !_cell.left;
        }

        if (_direction == Vector2Int.right)
        {
            return !_cell.right;
        }

        return false;
    }

    #endregion

    #region Wall Data

    /// <summary>
    /// Collects the wall information required to create
    /// the visible walls surrounding a specified cell.
    /// </summary>
    private List<WallData> GetWallsForCell(Vector2Int _position)
    {
        List<WallData> _walls = new List<WallData>();

        int _x = _position.x;
        int _y = _position.y;

        Cell _cell = _maze[_x, _y];

        Vector2 _center = _gridManager.GridToWorld(_position);

        float _halfSize = _gridManager.CellSize * 0.5f;
        float _cellSize = _gridManager.CellSize;

        // Top wall.
        if (_cell.top)
        {
            _walls.Add(new WallData(
                _center + new Vector2(0f, _halfSize),
                _cellSize,
                0f
            ));
        }

        // Right wall.
        if (_cell.right)
        {
            _walls.Add(new WallData(
                _center + new Vector2(_halfSize, 0f),
                _cellSize,
                90f
            ));
        }

        // Create the left boundary only for the first column.
        if (_x == 0 && _cell.left)
        {
            _walls.Add(new WallData(
                _center + new Vector2(-_halfSize, 0f),
                _cellSize,
                90f
            ));
        }

        // Create the bottom boundary only for the first row.
        if (_y == 0 && _cell.bottom)
        {
            _walls.Add(new WallData(
                _center + new Vector2(0f, -_halfSize),
                _cellSize,
                0f
            ));
        }

        return _walls;
    }

    #endregion

    #region Maze Reveal Animation

    /// <summary>
    /// Controls the complete wall creation and reveal sequence.
    /// Creates all walls in their hidden state, then starts the reveal animation.
    /// </summary>
    private IEnumerator AnimateMazeGeneration()
    {
        // Wait one frame so previously destroyed walls are removed.
        yield return null;

        List<WallRevealData> _revealWalls = CreateAllWallsForReveal();

        yield return StartCoroutine(
            RevealMazeWithPulse(_revealWalls)
        );

        _wallCreationCoroutine = null;
        _isRevealComplete = true;

        Debug.Log("Maze reveal completed!", this);
    }

    /// <summary>
    /// Creates all maze walls and collects the information
    /// required to reveal them progressively.
    /// </summary>
    private List<WallRevealData> CreateAllWallsForReveal()
    {
        List<WallRevealData> _revealWalls =
            new List<WallRevealData>();

        // The reveal wave begins at the first grid cell.
        Vector2 _startPosition =
            _gridManager.GridToWorld(Vector2Int.zero);

        for (int _x = 0; _x < _gridManager.Width; _x++)
        {
            for (int _y = 0; _y < _gridManager.Height; _y++)
            {
                Vector2Int _cellPosition = new Vector2Int(_x, _y);

                List<WallData> _walls = GetWallsForCell(_cellPosition);

                foreach (WallData _wall in _walls)
                {
                    GameObject _wallObject = CreateWall(
                        _wall.Position,
                        _wall.Length,
                        _wall.Rotation
                    );

                    SpriteRenderer[] _renderers =
                        _wallObject.GetComponentsInChildren<SpriteRenderer>(true);

                    // Calculate the wall's distance from the reveal starting point.
                    float _distance = Vector2.Distance(
                        _startPosition,
                        _wall.Position
                    );

                    WallRevealData _revealData =
                        new WallRevealData(_distance, _renderers);

                    _revealWalls.Add(_revealData);
                }
            }
        }

        return _revealWalls;
    }

    /// <summary>
    /// Reveals the maze walls using an expanding radial wave.
    /// Each wall gradually becomes visible according to its distance
    /// from the starting position.
    /// </summary>
    private IEnumerator RevealMazeWithPulse(
        List<WallRevealData> _revealWalls)
    {
        float _maxDistance = 0f;

        // Find the distance of the farthest wall from the starting position.
        foreach (WallRevealData _wall in _revealWalls)
        {
            if (_wall.Distance > _maxDistance)
            {
                _maxDistance = _wall.Distance;
            }
        }

        // Extend the wave beyond the farthest wall to complete the reveal.
        float _totalDistance = _maxDistance + _revealWaveWidth;

        float _elapsedTime = 0f;

        while (_elapsedTime < _revealDuration)
        {
            _elapsedTime += Time.deltaTime;

            // Calculate normalized progress over the animation duration.
            float _progress = Mathf.Clamp01(
                _elapsedTime / _revealDuration
            );

            // Smooth the animation progress for a gradual transition.
            float _smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                _progress
            );

            // Calculate the current radius of the expanding reveal wave.
            float _currentRadius = Mathf.Lerp(
                0f,
                _totalDistance,
                _smoothProgress
            );

            foreach (WallRevealData _wall in _revealWalls)
            {
                // Determine how much of this wall should be visible.
                float _revealAmount = Mathf.Clamp01(
                    (_currentRadius - _wall.Distance + _revealWaveWidth)
                    / _revealWaveWidth
                );

                _revealAmount = Mathf.SmoothStep(
                    0f,
                    1f,
                    _revealAmount
                );

                for (int _i = 0; _i < _wall.Renderers.Length; _i++)
                {
                    if (_wall.Renderers[_i] == null)
                    {
                        continue;
                    }

                    // Apply the reveal amount to the original alpha value.
                    Color _color = _wall.OriginalColors[_i];

                    _color.a *= _revealAmount;

                    _wall.Renderers[_i].color = _color;
                }
            }

            yield return null;
        }

        // Restore original colors when the animation finishes.
        foreach (WallRevealData _wall in _revealWalls)
        {
            for (int _i = 0; _i < _wall.Renderers.Length; _i++)
            {
                if (_wall.Renderers[_i] != null)
                {
                    _wall.Renderers[_i].color =
                        _wall.OriginalColors[_i];
                }
            }
        }
    }

    #endregion

    #region Immediate Wall Creation

    /// <summary>
    /// Creates all maze walls immediately without playing
    /// the progressive reveal animation.
    /// </summary>
    private void CreateMazeWallsImmediately()
    {
        PrepareWallParent();

        ClearExistingWalls();

        for (int _x = 0; _x < _gridManager.Width; _x++)
        {
            for (int _y = 0; _y < _gridManager.Height; _y++)
            {
                List<WallData> _walls = GetWallsForCell(
                    new Vector2Int(_x, _y)
                );

                foreach (WallData _wall in _walls)
                {
                    CreateWall(
                        _wall.Position,
                        _wall.Length,
                        _wall.Rotation
                    );
                }
            }
        }
    }

    #endregion

    #region Wall Creation

    /// <summary>
    /// Creates the parent GameObject used to organize
    /// all generated maze walls when no parent is assigned.
    /// </summary>
    private void PrepareWallParent()
    {
        if (_wallParent == null)
        {
            GameObject _parent = new GameObject("MazeWalls");

            _wallParent = _parent.transform;
        }
    }

    /// <summary>
    /// Removes all existing wall GameObjects
    /// from the assigned wall parent.
    /// </summary>
    private void ClearExistingWalls()
    {
        if (_wallParent == null)
        {
            return;
        }

        // Iterate backwards because child objects are being removed.
        for (int _i = _wallParent.childCount - 1; _i >= 0; _i--)
        {
            GameObject _child = _wallParent.GetChild(_i).gameObject;

            if (Application.isPlaying)
            {
                Destroy(_child);
            }
            else
            {
                DestroyImmediate(_child);
            }
        }
    }

    /// <summary>
    /// Instantiates a wall prefab at the specified position,
    /// applies its dimensions and rotation, and configures its visuals.
    /// </summary>
    private GameObject CreateWall(
        Vector2 _position,
        float _length,
        float _rotation)
    {
        GameObject _wall = Instantiate(
            _wallPrefab,
            _position,
            Quaternion.Euler(0f, 0f, _rotation),
            _wallParent
        );

        _wall.name = "MazeWall";

        // Extend the wall length to account for its thickness at the junctions.
        _wall.transform.localScale = new Vector3(
            _length + _wallThickness,
            _wallThickness,
            1f
        );

        MazeWallVisual _visual = _wall.GetComponent<MazeWallVisual>();

        if (_visual != null)
        {
            _visual.Apply();
        }
        else
        {
            Debug.LogWarning(
                "MazeWallVisual is missing on the wall prefab!",
                _wall
            );
        }

        return _wall;
    }

    #endregion

    #region Gizmos

    /// <summary>
    /// Draws the generated maze structure in the Unity Scene view
    /// using Gizmos to visualize the remaining walls of each cell.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_maze == null || _gridManager == null)
        {
            return;
        }

        if (!_gridManager.IsInitialized)
        {
            return;
        }

        // Ensure the maze dimensions still match the grid configuration.
        if (_maze.GetLength(0) != _gridManager.Width ||
            _maze.GetLength(1) != _gridManager.Height)
        {
            return;
        }

        Gizmos.color = Color.cyan;

        for (int _x = 0; _x < _gridManager.Width; _x++)
        {
            for (int _y = 0; _y < _gridManager.Height; _y++)
            {
                Vector2 _center = _gridManager.GridToWorld(
                    new Vector2Int(_x, _y)
                );

                float _halfSize = _gridManager.CellSize * 0.5f;

                Cell _cell = _maze[_x, _y];

                if (_cell.top)
                {
                    Gizmos.DrawLine(
                        _center + new Vector2(-_halfSize, _halfSize),
                        _center + new Vector2(_halfSize, _halfSize)
                    );
                }

                if (_cell.right)
                {
                    Gizmos.DrawLine(
                        _center + new Vector2(_halfSize, -_halfSize),
                        _center + new Vector2(_halfSize, _halfSize)
                    );
                }

                if (_cell.bottom)
                {
                    Gizmos.DrawLine(
                        _center + new Vector2(-_halfSize, -_halfSize),
                        _center + new Vector2(_halfSize, -_halfSize)
                    );
                }

                if (_cell.left)
                {
                    Gizmos.DrawLine(
                        _center + new Vector2(-_halfSize, -_halfSize),
                        _center + new Vector2(-_halfSize, _halfSize)
                    );
                }
            }
        }
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Maze Controller: MazeGenerator
/// Responsible for generating the maze data, managing wall GameObjects,
/// validating movement, and controlling the visual reveal sequence.
///
/// The Grid Dependency: GridManager
/// Provides the maze dimensions, cell size, grid-boundary validation,
/// and conversion between grid coordinates and world positions.
///
/// The Data Structure: Cell
/// Represents each individual maze cell.
/// Stores the state of its four walls and whether the cell has been visited.
///
/// The Generation Pipeline: GenerateMaze
/// Validates the required references, initializes the maze data,
/// generates passages using DFS, prepares the wall parent,
/// and starts either the animated or immediate wall creation process.
///
/// The Algorithm Pipeline: Depth-First Search
/// GenerateDFS explores the maze using a stack.
/// GetUnvisitedNeighbors finds available neighboring cells.
/// RemoveWall removes the shared wall between two adjacent cells.
/// When no unvisited neighbors remain, the algorithm backtracks
/// by popping the previous cell from the stack.
///
/// The Movement Validation Pipeline: CanMove
/// Checks whether a requested movement is valid.
/// Verifies the maze data, grid configuration, current cell,
/// movement direction, next cell, and the corresponding wall state.
///
/// The Wall Data Pipeline: GetWallsForCell
/// Converts the remaining walls of each cell into WallData.
/// Stores the world position, length, and rotation required
/// to create each wall GameObject.
///
/// The Wall Creation Pipeline: CreateWall
/// Instantiates the wall prefab, applies its dimensions and rotation,
/// and calls MazeWallVisual to configure its visual layers.
///
/// The Reveal Animation Pipeline: AnimateMazeGeneration
/// Creates the wall objects in a hidden state and starts
/// the progressive reveal animation.
///
/// The Reveal Calculation: RevealMazeWithPulse
/// Calculates the distance of each wall from the starting position.
/// An expanding wave determines when and how much each wall becomes visible.
/// SmoothStep provides gradual transitions, while the original colors
/// are restored after the animation finishes.
///
/// The Immediate Creation Pipeline: CreateMazeWallsImmediately
/// Creates all maze walls without a reveal animation.
/// Used when generating the maze outside Play Mode.
///
/// The Wall Organization: PrepareWallParent and ClearExistingWalls
/// Creates a parent object when needed and removes previous wall instances
/// before a new set of walls is generated.
///
/// The Debug Visualization: OnDrawGizmos
/// Draws the remaining walls of the generated maze in the Scene view,
/// allowing the maze structure to be inspected visually.
///
/// The Status Interface:
/// IsGenerated indicates whether maze data exists.
/// IsRevealComplete indicates whether the wall reveal animation has finished.
/// </summary>
