using System.Runtime.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private PlayerMovementSettings settings;
    private Rigidbody2D _rb;
    private Vector2 _mouvement;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        _mouvement.x = Input.GetAxisRaw("Horizontal");
        _mouvement.y = Input.GetAxisRaw("Vertical");

        _mouvement = Vector2.ClampMagnitude(_mouvement, 1f);
    }

    private void FixedUpdate()
    {
        if(settings == null)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }
        _rb.linearVelocity = _mouvement * settings._speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") &&
            GameManager.Instance != null)
        {
            GameManager.Instance.LoseGame();
        }
    }

}
