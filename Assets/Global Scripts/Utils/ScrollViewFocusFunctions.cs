using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollViewFocusFunctions
{
    public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint)
    {
        Vector2 contentSize = scrollView.content.rect.size;
        Vector2 viewportSize = ((RectTransform)scrollView.content.parent).rect.size;
        Vector2 contentScale = scrollView.content.localScale;

        contentSize.Scale(contentScale);
        focusPoint.Scale(contentScale);

        Vector2 scrollPosition = scrollView.normalizedPosition;
        if (scrollView.horizontal && contentSize.x > viewportSize.x)
            scrollPosition.x = Mathf.Clamp01((focusPoint.x - viewportSize.x * 0.5f) / (contentSize.x - viewportSize.x));
        if (scrollView.vertical && contentSize.y > viewportSize.y)
            scrollPosition.y = Mathf.Clamp01((focusPoint.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));

        return scrollPosition;
    }

    public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform item)
    {
        Vector2 itemCenterPoint = scrollView.content.InverseTransformPoint(item.transform.TransformPoint(item.rect.center));

        Vector2 contentSizeOffset = scrollView.content.rect.size;
        contentSizeOffset.Scale(scrollView.content.pivot);

        return scrollView.CalculateFocusedScrollPosition(itemCenterPoint + contentSizeOffset);
    }

    public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint)
    {
        scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
    }

    public static void FocusOnItem(this ScrollRect scrollView, RectTransform item)
    {
        scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);
    }

    private static async UniTask LerpToScrollPositionCoroutine(this ScrollRect scrollView, Vector2 targetNormalizedPos, float speed, CancellationToken cancellationToken)
    {
        Vector2 initialNormalizedPos = scrollView.normalizedPosition;

        float t = 0f;
        while (t < 1f)
        {
            scrollView.normalizedPosition = Vector2.LerpUnclamped(initialNormalizedPos, targetNormalizedPos, 1f - (1f - t) * (1f - t));

            await UniTask.NextFrame(cancellationToken);
            t += speed * Time.unscaledDeltaTime;
        }

        scrollView.normalizedPosition = targetNormalizedPos;
    }

    public static async UniTask FocusAtPointCoroutine(this ScrollRect scrollView, Vector2 focusPoint, float speed, CancellationToken cancellationToken = default)
    {
        await scrollView.LerpToScrollPositionCoroutine(scrollView.CalculateFocusedScrollPosition(focusPoint), speed, cancellationToken);
    }

    public static async UniTask FocusOnItemCoroutine(this ScrollRect scrollView, RectTransform item, float speed, CancellationToken cancellationToken = default)
    {
        await scrollView.LerpToScrollPositionCoroutine(scrollView.CalculateFocusedScrollPosition(item), speed, cancellationToken);
    }
}