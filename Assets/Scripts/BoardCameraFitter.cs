using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class BoardCameraFitter : MonoBehaviour
{
    [Header("References")]
    public BoardManager board;
    public Tilemap referenceTilemap;

    [Header("Padding")]
    [Tooltip("Extra space around the board in world units.")]
    public float padding = 0.25f;

    [Tooltip("Optional extra vertical space if you want a little room above the board.")]
    public float topPadding = 0f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (board == null)
            board = FindObjectOfType<BoardManager>();

        if (referenceTilemap == null && board != null)
            referenceTilemap = board.floorTilemap;
    }

    public void FitToBoard()
    {
        if (cam == null || board == null || referenceTilemap == null || board.level == null)
            return;

        int width = board.level.Width;
        int height = board.level.Height;

        if (width <= 0 || height <= 0)
            return;

        // Bottom-left world corner of cell (0,0)
        Vector3 min = referenceTilemap.CellToWorld(new Vector3Int(0, 0, 0));

        // Top-right world corner just outside the board
        Vector3 max = referenceTilemap.CellToWorld(new Vector3Int(width, height, 0));

        float boardWorldWidth = max.x - min.x;
        float boardWorldHeight = max.y - min.y;

        float centerX = min.x + (boardWorldWidth * 0.5f);
        float centerY = min.y + (boardWorldHeight * 0.5f);

        // Center camera on board
        Vector3 camPos = transform.position;
        transform.position = new Vector3(centerX, centerY, camPos.z);

        // Orthographic size is half the visible height
        float verticalSize = (boardWorldHeight * 0.5f) + padding + topPadding;
        float horizontalSize = (boardWorldWidth * 0.5f) / cam.aspect + padding;

        cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }
}