
using UnityEngine;

public class MazeWallVisual : MonoBehaviour
{
    #region Inspector Fields

    [Header("Visual References")]

    [SerializeField] private SpriteRenderer glow;
    [SerializeField] private SpriteRenderer edge;
    [SerializeField] private SpriteRenderer body;


    [Header("Visual Dimensions")]

    [Tooltip("Thickness of the cyan border in world units.")]
    public float edgeWidth = 0.02f;

    [Tooltip("Extension of the glow beyond the wall.")]
    public float glowWidth = 0.05f;

    #endregion


    #region Initialization

    private void Awake()
    {
        if (glow == null)
        {
            glow = FindLayer("Glow");
        }

        if (edge == null)
        {
            edge = FindLayer("Edge");
        }

        if (body == null)
        {
            body = FindLayer("Body");
        }
    }

    #endregion


    #region Visual Configuration

    public void Apply()
    {
        float wallLength = transform.localScale.x;
        float wallThickness = transform.localScale.y;

        float border = Mathf.Min(
            edgeWidth,
            wallThickness * 0.35f
        );

        // Glow: largest layer.
        Fit(
            glow,
            wallLength + (2f * glowWidth),
            wallThickness + (2f * glowWidth)
        );

        // Edge: outer cyan border.
        Fit(
            edge,
            wallLength,
            wallThickness
        );

        // Body: smaller inner layer.
        Fit(
            body,
            wallLength - (2f * border),
            wallThickness - (2f * border)
        );
    }


    private void Fit(
        SpriteRenderer spriteRenderer,
        float worldWidth,
        float worldHeight
    )
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (spriteRenderer.sprite == null)
        {
            Debug.LogWarning(
                spriteRenderer.name + " has no Sprite assigned!",
                spriteRenderer
            );

            return;
        }

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector3 rootScale = transform.localScale;

        Transform childTransform = spriteRenderer.transform;

        childTransform.localPosition = Vector3.zero;
        childTransform.localRotation = Quaternion.identity;

        childTransform.localScale = new Vector3(
            worldWidth / (rootScale.x * spriteSize.x),
            worldHeight / (rootScale.y * spriteSize.y),
            1f
        );
    }

    #endregion


    #region Helper Methods

    private SpriteRenderer FindLayer(string layerName)
    {
        Transform child = transform.Find(layerName);

        if (child == null)
        {
            Debug.LogWarning(
                "Could not find " + layerName + " inside MazeWall!",
                this
            );

            return null;
        }

        return child.GetComponent<SpriteRenderer>();
    }

    #endregion
}
