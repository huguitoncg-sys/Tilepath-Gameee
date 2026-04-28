using UnityEngine;
using UnityEngine.UI;

public class UIWaterReflectionCycler : MonoBehaviour
{
    [Header("Water Reflection UI Images")]
    public GameObject[] reflectionImages;

    [Header("Animation Settings")]
    public float framesPerSecond = 6f;
    public bool useUnscaledTime = true;

    private int currentFrame = 0;
    private float timer = 0f;

    private static int savedFrame = 0;
    private static float savedTimer = 0f;
    private static bool hasStartedBefore = false;

    private void Start()
    {
        if (reflectionImages == null || reflectionImages.Length == 0)
        {
            Debug.LogWarning("No water reflection images assigned.");
            enabled = false;
            return;
        }

        if (hasStartedBefore)
        {
            currentFrame = savedFrame;
            timer = savedTimer;
        }
        else
        {
            hasStartedBefore = true;
            currentFrame = 0;
            timer = 0f;
        }

        ShowOnlyCurrentFrame();
    }

    private void Update()
    {
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        timer += deltaTime;

        float secondsPerFrame = 1f / framesPerSecond;

        while (timer >= secondsPerFrame)
        {
            timer -= secondsPerFrame;
            currentFrame++;

            if (currentFrame >= reflectionImages.Length)
            {
                currentFrame = 0;
            }
        }

        savedFrame = currentFrame;
        savedTimer = timer;

        ShowOnlyCurrentFrame();
    }

    private void ShowOnlyCurrentFrame()
    {
        for (int i = 0; i < reflectionImages.Length; i++)
        {
            if (reflectionImages[i] != null)
            {
                reflectionImages[i].SetActive(i == currentFrame);
            }
        }
    }
}