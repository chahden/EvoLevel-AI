
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]

    [SerializeField] private PlayerMovementSettings settings;

    [SerializeField] private GridManager gridManager;

    [SerializeField] private MazeGenerator mazeGenerator;

    #endregion


    #region Private Variables

    private Rigidbody2D _rb;

    private Vector2 _targetPosition;

    private bool _isMoving;

    private bool _canMove = false;

    #endregion


    #region Unity Lifecycle

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        _targetPosition = _rb.position;

        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
        }

        if (mazeGenerator == null)
        {
            mazeGenerator = FindFirstObjectByType<MazeGenerator>();
        }

        if (gridManager == null)
        {
            Debug.LogError("GridManager could not be found!", this);
        }

        if (mazeGenerator == null)
        {
            Debug.LogError("MazeGenerator could not be found!", this);
        }
    }


    private void Update()
    {
        if (!_canMove)
        {
            return;
        }

        if (_isMoving)
        {
            return;
        }

        if (GameManager.Instance != null &&
            GameManager.Instance.GameEnded)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2Int direction = Vector2Int.zero;

        if (horizontal != 0)
        {
            direction.x = (int)horizontal;
        }
        else if (vertical != 0)
        {
            direction.y = (int)vertical;
        }

        if (direction != Vector2Int.zero)
        {
            TryMove(direction);
        }
    }


    private void FixedUpdate()
    {
        if (!_isMoving)
        {
            return;
        }

        if (settings == null)
        {
            Debug.LogError("PlayerMovementSettings is missing!", this);

            _isMoving = false;
            return;
        }

        Vector2 newPosition = Vector2.MoveTowards(
            _rb.position,
            _targetPosition,
            settings._speed * Time.fixedDeltaTime
        );

        _rb.MovePosition(newPosition);

        if (Vector2.Distance(newPosition, _targetPosition) < 0.01f)
        {
            _rb.position = _targetPosition;

            _isMoving = false;
        }
    }

    #endregion


    #region Movement Methods

    public void SetMovementEnabled(bool enabled)
    {
        _canMove = enabled;
    }


    private void TryMove(Vector2Int direction)
    {
        if (gridManager == null || mazeGenerator == null)
        {
            return;
        }

        Vector2Int currentCell =
            gridManager.WorldToGrid(_rb.position);

        if (!mazeGenerator.CanMove(currentCell, direction))
        {
            return;
        }

        Vector2Int nextCell = currentCell + direction;

        _targetPosition = gridManager.GridToWorld(nextCell);

        _isMoving = true;
    }

    #endregion
}
