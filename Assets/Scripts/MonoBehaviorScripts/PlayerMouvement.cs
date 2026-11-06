using System.Runtime.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Inspector Field
        [Header("References")]
            [Tooltip("Settings asset containing speed and movement parameters.")]
                [SerializeField] private PlayerMovementSettings settings = null;
            [Tooltip("Reference to the GridManager handling grid boundaries and coordinates.")]
                [SerializeField] private GridManager gridManager;
    #endregion

    #region Private Variables
        private Rigidbody2D _rb;
        private Vector2 _targetPosition;
        private bool _isMoving ;
    #endregion

    #region Unity Lifecycle
        private void Awake()
        {
           _rb = GetComponent<Rigidbody2D>();
           _rb.gravityScale = 0f;
           _rb.freezeRotation = true;
           _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
           _targetPosition = _rb.position; 
        }
        private void Update()
        {
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
            if (!_isMoving) return; 
            if(settings == null)
            {
                Debug.LogError("PlayerMovementSettings is missing!", this);   
                _isMoving = false;
                return;             
            }
            Vector2 newPosition = Vector2.MoveTowards(_rb.position, _targetPosition, settings._speed * Time.deltaTime);
            _rb.MovePosition(newPosition);
            if(Vector2.Distance(newPosition, _targetPosition) < 0.1f)
            {
                _rb.position = _targetPosition;
                _isMoving = false;
            }         
        }
    #endregion

    #region Movement Methods

    /// <summary>
    /// Evaluates directional input, checks grid boundaries and obstacles, 
    /// and initiates smooth movement towards the target cell if valid.
    /// </summary>
    
    
        private void TryMove(Vector2Int direction)
        {
           if(gridManager == null)
            {
                Debug.LogError("GridManager is missing!", this);
                return;    
            }
            Vector2Int currentCell = gridManager.WorldToGrid(_rb.position);
            Vector2Int nextCell = currentCell + direction;

            if(!gridManager.IsInsideGrid(nextCell)) return;

            Vector2 nextPosition = gridManager.WorldToGrid(nextCell);
            float checkSize = gridManager._cellSize * 0.8f;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(
                nextPosition,
                new Vector2(checkSize, checkSize),
                0f
            );
            
            foreach (Collider2D hit  in colliders)
            {
                if (hit.CompareTag("Obstacle"))
                {
                    return;
                }
            }
            
            _targetPosition = nextPosition;
            _isMoving = true;
            
        }
    #endregion
}
