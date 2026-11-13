
public class LevelConfiguration
{
    #region Grid Configuration

    private int _width;
    private int _height;
    private float _cellSize;

    #endregion


    #region Level Positions

    private UnityEngine.Vector2Int _startCell;
    private UnityEngine.Vector2Int _goalCell;

    #endregion


    #region Properties

    public int Width { get { return _width; } }
    public int Height { get { return _height; } }
    public float CellSize { get { return _cellSize; } }

    public UnityEngine.Vector2Int StartCell { get { return _startCell; } }
    public UnityEngine.Vector2Int GoalCell { get { return _goalCell; } }

    #endregion


    #region Constructor

    public LevelConfiguration(
        int _width,
        int _height,
        float _cellSize,
        UnityEngine.Vector2Int _startCell,
        UnityEngine.Vector2Int _goalCell
    )
    {
        this._width = _width;
        this._height = _height;
        this._cellSize = _cellSize;

        this._startCell = _startCell;
        this._goalCell = _goalCell;
    }

    #endregion
}
