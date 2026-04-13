using UnityEngine;
using UnityEngine.Tilemaps;

public class SnakeFacingOnSpawn : MonoBehaviour
{
    [Header("Assign your board Tilemap here")]
    [SerializeField] private Tilemap boardTilemap;

    [Header("Renderer for the snake head")]
    [SerializeField] private SpriteRenderer headRenderer;

    // If your head points LEFT by default in the sprite, keep this true.
    [SerializeField] private bool spriteFacesLeftByDefault = true;

    private void Start()
    {
        SetInitialFacingTowardBoardCenter();
    }

    private void SetInitialFacingTowardBoardCenter()
    {
        if (boardTilemap == null || headRenderer == null) return;

        // Spawn cell
        Vector3Int headCell = boardTilemap.WorldToCell(transform.position);

        // Board center cell (based on tilemap bounds)
        BoundsInt b = boardTilemap.cellBounds;
        int centerX = b.xMin + (b.size.x - 1) / 2;
        int centerY = b.yMin + (b.size.y - 1) / 2;

        int dx = centerX - headCell.x;
        int dy = centerY - headCell.y;

        Vector2Int dir;
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            dir = new Vector2Int(dx > 0 ? 1 : -1, 0);      // right/left
        else
            dir = new Vector2Int(0, dy > 0 ? 1 : -1);      // up/down

        ApplyFacing(dir);
    }

    private void ApplyFacing(Vector2Int dir)
    {
        // This assumes your head sprite is drawn pointing LEFT by default.
        // We handle left/right with flipX, and up/down with rotation.
        // If your art is different, tell me which direction the head sprite points by default.

        // Reset first
        headRenderer.flipX = false;
        transform.rotation = Quaternion.identity;

        if (dir.x != 0)
        {
            // Horizontal
            bool faceRight = dir.x > 0;

            if (spriteFacesLeftByDefault)
                headRenderer.flipX = faceRight;   // left default -> flip to face right
            else
                headRenderer.flipX = !faceRight;  // right default -> flip to face left
        }
        else
        {
            // Vertical (rotate 90 degrees)
            bool faceUp = dir.y > 0;

            // If sprite points LEFT by default, rotating -90 makes it face UP, +90 makes it face DOWN.
            float z = faceUp ? -90f : 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, z);
        }
    }
}
