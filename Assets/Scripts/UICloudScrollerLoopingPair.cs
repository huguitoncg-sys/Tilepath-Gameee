using UnityEngine;

public class UICloudScrollerLoopingPair : MonoBehaviour
{
    [Header("Assign Only One Cloud")]
    public RectTransform cloudPrefab;

    [Header("Movement")]
    public float scrollSpeed = 60f;

    [Header("Spacing")]
    public float distanceBetweenClouds = 550f;

    [Header("Loop Padding")]
    public float extraPadding = 40f;

    private RectTransform parentRect;
    private RectTransform cloudA;
    private RectTransform cloudB;

    private float spawnX;
    private float despawnX;

    private static bool hasGlobalStartTime;
    private static float globalStartTime;

    private void Awake()
    {
        if (cloudPrefab == null)
        {
            Debug.LogError("UICloudScrollerLoopingPair needs a cloud prefab/reference.");
            enabled = false;
            return;
        }

        parentRect = cloudPrefab.parent as RectTransform;

        if (parentRect == null)
        {
            Debug.LogError("Cloud must be inside a Canvas or UI object with a RectTransform.");
            enabled = false;
            return;
        }

        if (!hasGlobalStartTime)
        {
            hasGlobalStartTime = true;
            globalStartTime = Time.realtimeSinceStartup;
        }

        cloudA = cloudPrefab;

        GameObject duplicate = Instantiate(cloudPrefab.gameObject, cloudPrefab.parent);
        duplicate.name = cloudPrefab.name + " Duplicate";
        cloudB = duplicate.GetComponent<RectTransform>();

        CalculateEdges();
    }

    private void Update()
    {
        CalculateEdges();

        float elapsed = Time.realtimeSinceStartup - globalStartTime;

        MoveCloud(cloudA, elapsed, 0f);
        MoveCloud(cloudB, elapsed, distanceBetweenClouds);
    }

    private void MoveCloud(RectTransform cloud, float elapsed, float offset)
    {
        float totalTravelDistance = spawnX - despawnX;

        float distanceMoved = elapsed * scrollSpeed + offset;
        float wrappedDistance = Mathf.Repeat(distanceMoved, totalTravelDistance);

        float newX = spawnX - wrappedDistance;

        cloud.anchoredPosition = new Vector2(
            newX,
            cloud.anchoredPosition.y
        );
    }

    private void CalculateEdges()
    {
        float parentLeftEdge = -parentRect.rect.width * parentRect.pivot.x;
        float parentRightEdge = parentRect.rect.width * (1f - parentRect.pivot.x);

        float cloudHalfWidth = Mathf.Abs(cloudPrefab.rect.width * cloudPrefab.localScale.x) / 2f;

        spawnX = parentRightEdge + cloudHalfWidth + extraPadding;
        despawnX = parentLeftEdge - cloudHalfWidth - extraPadding;
    }
}