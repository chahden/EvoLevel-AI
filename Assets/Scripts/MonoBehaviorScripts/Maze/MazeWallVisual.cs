
using UnityEngine;

/// <summary>
/// Manages the visual appearance of maze walls
/// by configuring their glow, edge, and body sprite layers.
/// </summary>
public class MazeWallVisual : MonoBehaviour
{
    #region Inspector Fields

    [Header("Visual References")]

    [Tooltip("Sprite renderer used for the outer glow layer.")]
    [SerializeField] private SpriteRenderer _glow;

    [Tooltip("Sprite renderer used for the outer edge layer.")]
    [SerializeField] private SpriteRenderer _edge;

    [Tooltip("Sprite renderer used for the inner body layer.")]
    [SerializeField] private SpriteRenderer _body;

    [Header("Visual Dimensions")]

    [Tooltip("Thickness of the cyan border in world units.")]
    public float edgeWidth = 0.02f;

    [Tooltip("Extension of the glow beyond the wall.")]
    public float glowWidth = 0.05f;

    #endregion

    #region Initialization

    private void Awake()
    {
        // Automatically find visual layers if they were not assigned in the Inspector.
        if (_glow == null)
        {
            _glow = FindLayer("Glow");
        }

        if (_edge == null)
        {
            _edge = FindLayer("Edge");
        }

        if (_body == null)
        {
            _body = FindLayer("Body");
        }
    }

    #endregion

    #region Visual Configuration

    /// <summary>
    /// Adjusts the dimensions of each visual layer
    /// according to the wall's current scale.
    /// </summary>
    public void Apply()
    {
        float _wallLength = transform.localScale.x;
        float _wallThickness = transform.localScale.y;

        // Prevent the border from becoming too thick compared to the wall.
        float _border = Mathf.Min(
            edgeWidth,
            _wallThickness * 0.35f
        );

        // Glow: largest layer, extending beyond the wall.
        Fit(
            _glow,
            _wallLength + (2f * glowWidth),
            _wallThickness + (2f * glowWidth)
        );

        // Edge: outer cyan border matching the wall dimensions.
        Fit(
            _edge,
            _wallLength,
            _wallThickness
        );

        // Body: smaller inner layer, leaving space for the border.
        Fit(
            _body,
            _wallLength - (2f * _border),
            _wallThickness - (2f * _border)
        );
    }

    /// <summary>
    /// Resizes a sprite renderer so its visual dimensions
    /// match the requested world-space width and height.
    /// </summary>
    private void Fit(
        SpriteRenderer _spriteRenderer,
        float _worldWidth,
        float _worldHeight
    )
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        if (_spriteRenderer.sprite == null)
        {
            Debug.LogWarning(
                _spriteRenderer.name + " has no Sprite assigned!",
                _spriteRenderer
            );

            return;
        }

        // Retrieve the original dimensions of the assigned sprite.
        Vector2 _spriteSize = _spriteRenderer.sprite.bounds.size;

        Vector3 _rootScale = transform.localScale;

        Transform _childTransform = _spriteRenderer.transform;

        // Reset the child transform so its size is calculated consistently.
        _childTransform.localPosition = Vector3.zero;
        _childTransform.localRotation = Quaternion.identity;

        // Calculate the local scale needed to achieve the requested world dimensions.
        _childTransform.localScale = new Vector3(
            _worldWidth / (_rootScale.x * _spriteSize.x),
            _worldHeight / (_rootScale.y * _spriteSize.y),
            1f
        );
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Finds a visual layer by its child object name
    /// and retrieves its SpriteRenderer component.
    /// </summary>
    private SpriteRenderer FindLayer(string _layerName)
    {
        Transform _child = transform.Find(_layerName);

        if (_child == null)
        {
            Debug.LogWarning(
                "Could not find " + _layerName + " inside MazeWall!",
                this
            );

            return null;
        }

        return _child.GetComponent<SpriteRenderer>();
    }

    #endregion
}

/// <summary>
/// ARCHITECTURAL OVERVIEW
///
/// The Visual Controller: MazeWallVisual
/// Manages the appearance of an individual maze wall
/// using three visual layers: Glow, Edge, and Body.
///
/// The Initialization Pipeline: Awake
/// Checks whether the visual references have been assigned
/// through the Inspector. If a reference is missing,
/// FindLayer attempts to retrieve it from the wall's children.
///
/// The Visual Configuration Pipeline: Apply
/// Reads the wall's dimensions and calculates the appropriate
/// dimensions for each visual layer.
///
/// The Visual Layers:
/// Glow represents the outer visual effect and extends beyond the wall.
/// Edge represents the outer border of the wall.
/// Body represents the smaller inner section of the wall.
///
/// The Scaling Pipeline: Fit
/// Calculates the required local scale of each SpriteRenderer
/// using its original sprite dimensions and the root wall scale.
/// This allows the visual layers to match the requested
/// world-space dimensions.
///
/// The Helper Method: FindLayer
/// Searches for a child object using its name
/// and retrieves its SpriteRenderer component.
///
/// The Configuration Relationship:
/// MazeGenerator creates the wall GameObject and calls Apply()
/// to configure its visual layers according to the wall dimensions.
/// </summary>
