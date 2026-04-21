using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [SerializeField] private float scrollSpeed = 0.15f;

    [Header("Direction")]
    [SerializeField] private bool moveDown = true;

    private Renderer backgroundRenderer;
    private Vector2 textureOffset;

    private void Awake()
    {
        backgroundRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        float direction = moveDown ? -1f : 1f;

        textureOffset.y += direction * scrollSpeed * Time.deltaTime;

        backgroundRenderer.material.mainTextureOffset = textureOffset;
    }
}