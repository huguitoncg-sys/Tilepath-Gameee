using UnityEngine;
using UnityEngine.UI;

public class WorldSelectButton : MonoBehaviour
{
    [Header("World Info")]
    public WorldData worldData;

    [Header("UI")]
    public Image targetImage;

    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite completedSprite;

    private void OnEnable()
    {
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (targetImage == null || worldData == null)
            return;

        bool completed = GameManager.Instance != null && GameManager.Instance.IsWorldCompleted(worldData);

        if (completed && completedSprite != null)
            targetImage.sprite = completedSprite;
        else if (defaultSprite != null)
            targetImage.sprite = defaultSprite;
    }
}