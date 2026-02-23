using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class HoverScale : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.12f;
    [SerializeField] private float duration = 0.12f;

    [Header("Optional")]
    [SerializeField] private bool alsoOnSelect = true;

    private RectTransform rect;
    private Vector3 originalScale;
    private Coroutine anim;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale; // store ONCE
    }

    private void OnEnable()
    {
        // Always reset when the object becomes active again
        ResetScaleImmediate();
    }

    private void OnDisable()
    {
        // Also reset when leaving/hiding the panel (exit might not fire)
        ResetScaleImmediate();
    }

    private void ResetScaleImmediate()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (anim != null) StopCoroutine(anim);
        anim = null;

        rect.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateTo(originalScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateTo(originalScale);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (alsoOnSelect) AnimateTo(originalScale * hoverScale);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (alsoOnSelect) AnimateTo(originalScale);
    }

    private void AnimateTo(Vector3 targetScale)
    {
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(ScaleRoutine(targetScale));
    }

    private IEnumerator ScaleRoutine(Vector3 target)
    {
        Vector3 start = rect.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            p = p * p * (3f - 2f * p); // smoothstep
            rect.localScale = Vector3.LerpUnclamped(start, target, p);
            yield return null;
        }

        rect.localScale = target;
        anim = null;
    }
}
