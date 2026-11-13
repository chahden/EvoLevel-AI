
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MazeGenerator : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]

    [FormerlySerializedAs("gridManager")]
    [SerializeField] private GridManager _gridManager;

    [Header("Wall Settings")]

    [FormerlySerializedAs("wallPrefab")]
    [SerializeField] private GameObject _wallPrefab;

    [FormerlySerializedAs("wallParent")]
    [SerializeField] private Transform _wallParent;

    [FormerlySerializedAs("wallThickness")]
    [SerializeField] private float _wallThickness = 0.12f;

    [Header("Maze Reveal Animation")]

    [FormerlySerializedAs("revealDuration")]
    [Min(0.1f)]
    [SerializeField] private float _revealDuration = 1.2f;

    [FormerlySerializedAs("revealWaveWidth")]
    [Min(0.1f)]
    [SerializeField] private float _revealWaveWidth = 2f;

    #endregion

    #region Cell Structure

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

                Color _hiddenColor = OriginalColors[_i];
                _hiddenColor.a = 0f;

                Renderers[_i].color = _hiddenColor;
            }
        }
    }

    private Coroutine _wallCreationCoroutine;
    private bool _isRevealComplete;

    public bool IsRevealComplete
    {
        get { return _isRevealComplete; }
    }

    public bool IsGenerated
    {
        get { return _maze != null; }
    }

    #endregion

    #region Maze Generation

[ContextMenu("Generate Maze")]

public void GenerateMaze()
{
    if (!ValidateReferences())
    {
        return;
    }

    // The reveal is not complete while a new maze is being generated.
    _isRevealComplete = false;

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
                _maze[_x, _y] = new Cell();
            }
        }
    }

    #endregion

    #region Depth First Search

    private void GenerateDFS()
    {
        Stack<Vector2Int> _stack = new Stack<Vector2Int>();

        Vector2Int _currentCell = Vector2Int.zero;

        _maze[_currentCell.x, _currentCell.y].visited = true;

        _stack.Push(_currentCell);

        while (_stack.Count > 0)
        {
            _currentCell = _stack.Peek();

            List<Vector2Int> _neighbors =
                GetUnvisitedNeighbors(_currentCell);

            if (_neighbors.Count > 0)
            {
                int _randomIndex = Random.Range(0, _neighbors.Count);

                Vector2Int _nextCell = _neighbors[_randomIndex];

                RemoveWall(_currentCell, _nextCell);

                _maze[_nextCell.x, _nextCell.y].visited = true;

                _stack.Push(_nextCell);
            }
            else
            {
                _stack.Pop();
            }
        }
    }

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

            if (!_gridManager.IsInsideGrid(_neighbor))
            {
                continue;
            }

            if (!_maze[_neighbor.x, _neighbor.y].visited)
            {
                _neighbors.Add(_neighbor);
            }
        }

        return _neighbors;
    }

    private void RemoveWall(Vector2Int _current, Vector2Int _next)
    {
        Vector2Int _direction = _next - _current;

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

        if (_maze.GetLength(0) != _gridManager.Width ||
            _maze.GetLength(1) != _gridManager.Height)
        {
            return false;
        }

        if (!_gridManager.IsInsideGrid(_currentCell))
        {
            return false;
        }

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

        if (!_gridManager.IsInsideGrid(_nextCell))
        {
            return false;
        }

        Cell _cell = _maze[_currentCell.x, _currentCell.y];

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

    private List<WallData> GetWallsForCell(Vector2Int _position)
    {
        List<WallData> _walls = new List<WallData>();

        int _x = _position.x;
        int _y = _position.y;

        Cell _cell = _maze[_x, _y];

        Vector2 _center = _gridManager.GridToWorld(_position);

        float _halfSize = _gridManager.CellSize * 0.5f;
        float _cellSize = _gridManager.CellSize;

        // Top wall
        if (_cell.top)
        {
            _walls.Add(new WallData(
                _center + new Vector2(0f, _halfSize),
                _cellSize,
                0f
            ));
        }

        // Right wall
        if (_cell.right)
        {
            _walls.Add(new WallData(
                _center + new Vector2(_halfSize, 0f),
                _cellSize,
                90f
            ));
        }

        // Left boundary
        if (_x == 0 && _cell.left)
        {
            _walls.Add(new WallData(
                _center + new Vector2(-_halfSize, 0f),
                _cellSize,
                90f
            ));
        }

        // Bottom boundary
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

    private List<WallRevealData> CreateAllWallsForReveal()
    {
        List<WallRevealData> _revealWalls =
            new List<WallRevealData>();

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

    private IEnumerator RevealMazeWithPulse(
        List<WallRevealData> _revealWalls)
    {
        float _maxDistance = 0f;

        foreach (WallRevealData _wall in _revealWalls)
        {
            if (_wall.Distance > _maxDistance)
            {
                _maxDistance = _wall.Distance;
            }
        }

        float _totalDistance = _maxDistance + _revealWaveWidth;

        float _elapsedTime = 0f;

        while (_elapsedTime < _revealDuration)
        {
            _elapsedTime += Time.deltaTime;

            float _progress = Mathf.Clamp01(
                _elapsedTime / _revealDuration
            );

            float _smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                _progress
            );

            float _currentRadius = Mathf.Lerp(
                0f,
                _totalDistance,
                _smoothProgress
            );

            foreach (WallRevealData _wall in _revealWalls)
            {
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

    private void PrepareWallParent()
    {
        if (_wallParent == null)
        {
            GameObject _parent = new GameObject("MazeWalls");

            _wallParent = _parent.transform;
        }
    }

    private void ClearExistingWalls()
    {
        if (_wallParent == null)
        {
            return;
        }

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
