using UnityEngine;

public class UICloudScrollerSingle : MonoBehaviour
{
    [Header("Cloud UI Object")]
    public RectTransform cloud;

    [Header("Movement")]
    public float scrollSpeed = 60f;

    [Header("Loop Padding")]
    public float extraPadding = 20f;

    [Header("Optional")]
    public bool useUnscaledTime = true;

    private RectTransform parentRect;

    private static bool hasStartedBefore = false;
    private static float firstStartTime;
    private static float savedPhaseOffset;

    private float spawnX;
    private float despawnX;
    private float travelDistance;
    private float cloudHalfWidth;

    private void Awake()
    {
        if (cloud == null)
        {
            Debug.LogError("UICloudScrollerSingle is missing the cloud RectTransform.");
            enabled = false;
            return;
        }

        parentRect = cloud.parent as RectTransform;

        if (parentRect == null)
        {
            Debug.LogError("The cloud must be inside a Canvas or UI parent with a RectTransform.");
            enabled = false;
            return;
        }

        CalculateLoopPoints();

        if (!hasStartedBefore)
        {
            hasStartedBefore = true;
            firstStartTime = Time.realtimeSinceStartup;

            // This keeps the cloud starting from wherever you placed it in the scene.
            savedPhaseOffset = spawnX - cloud.anchoredPosition.x;
        }
    }

    private void Update()
    {
        CalculateLoopPoints();

        float elapsed;

        if (useUnscaledTime)
        {
            elapsed = Time.realtimeSinceStartup - firstStartTime;
        }
        else
        {
            elapsed = Time.time - firstStartTime;
        }

        float distanceMoved = elapsed * scrollSpeed + savedPhaseOffset;
        float wrappedDistance = Mathf.Repeat(distanceMoved, travelDistance);

        float newX = spawnX - wrappedDistance;

        cloud.anchoredPosition = new Vector2(
            newX,
            cloud.anchoredPosition.y
        );
    }

    private void CalculateLoopPoints()
    {
        float parentLeftEdge = -parentRect.rect.width * parentRect.pivot.x;
        float parentRightEdge = parentRect.rect.width * (1f - parentRect.pivot.x);

        cloudHalfWidth = Mathf.Abs(cloud.rect.width * cloud.localScale.x) / 2f;

        // Cloud appears fully off the right side first.
        spawnX = parentRightEdge + cloudHalfWidth + extraPadding;

        // Cloud only loops after it is fully off the left side.
        despawnX = parentLeftEdge - cloudHalfWidth - extraPadding;

        travelDistance = spawnX - despawnX;

        if (travelDistance <= 0f)
        {
            travelDistance = 1f;
        }
    }
}