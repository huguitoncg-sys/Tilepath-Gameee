using System.Collections;
using UnityEngine;

public class DropDownUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform panelToMove;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Drop Settings")]
    [SerializeField] private float dropDuration = 0.25f;
    [SerializeField] private float hiddenY = 900f;
    [SerializeField] private float shownY = 0f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        HideInstant();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(DropIn());
    }

    public void HideInstant()
    {
        if (panelToMove != null)
        {
            Vector2 pos = panelToMove.anchoredPosition;
            pos.y = hiddenY;
            panelToMove.anchoredPosition = pos;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        gameObject.SetActive(false);
    }

    public void Hide()
    {
        HideInstant();
    }

    private IEnumerator DropIn()
    {
        if (panelToMove == null)
            yield break;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        Vector2 startPos = panelToMove.anchoredPosition;
        startPos.y = hiddenY;

        Vector2 endPos = panelToMove.anchoredPosition;
        endPos.y = shownY;

        panelToMove.anchoredPosition = startPos;

        float timer = 0f;

        while (timer < dropDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / dropDuration;

            // Smooth drop with slight ease-out
            t = 1f - Mathf.Pow(1f - t, 3f);

            panelToMove.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        panelToMove.anchoredPosition = endPos;
    }
}