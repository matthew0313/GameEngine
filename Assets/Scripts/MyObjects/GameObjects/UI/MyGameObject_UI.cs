using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class MyGameObject_UI : MyGameObject
{
    protected RectTransform rectTransform;
    private void OnEnable()
    {
        rectTransform ??= GetComponent<RectTransform>();
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        yield return new ExposedString(
            "Name",
            () => name,
            (value) => name = value);
        yield return new ExposedAnchor(
            () => rectTransform.anchorMin,
            () => rectTransform.anchorMax,
            (value) => rectTransform.anchorMin = value,
            (value) => rectTransform.anchorMax = value);
        if (rectTransform.anchorMin.x == rectTransform.anchorMax.x)
        {
            yield return new ExposedNumber(
                "Pos X",
                () => rectTransform.anchoredPosition.x,
                (value) => rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y));
            yield return new ExposedNumber(
                "Width",
                () => rectTransform.rect.width,
                (value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value));
        }
        else
        {
            yield return new ExposedNumber(
                "Left",
                () => rectTransform.offsetMin.x,
                (value) => rectTransform.offsetMin = new Vector2(value, rectTransform.offsetMin.y));
            yield return new ExposedNumber(
                "Right",
                () => -rectTransform.offsetMax.x,
                (value) => rectTransform.offsetMax = new Vector2(-value, rectTransform.offsetMax.y));
        }
        if (rectTransform.anchorMin.y == rectTransform.anchorMax.y)
        {
            yield return new ExposedNumber(
                "Pos Y",
                () => rectTransform.anchoredPosition.y,
                (value) => rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, value));
            yield return new ExposedNumber(
                "Height",
                () => rectTransform.rect.height,
                (value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value));
        }
        else
        {
            yield return new ExposedNumber(
                "Top",
                () => -rectTransform.offsetMax.y,
                (value) => rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -value));
            yield return new ExposedNumber(
                "Bottom",
                () => rectTransform.offsetMin.y,
                (value) => rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, value));
        }
        yield return new ExposedVector2(
            "Pivot",
            () => rectTransform.pivot,
            (value) => rectTransform.pivot = value);
        yield return new ExposedNumber(
            "Rotation",
            () => transform.localEulerAngles.z,
            (value) => transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value));
    }
}
public class ExposedAnchor : ExposedElement
{
    public readonly Func<Vector2> minGetter, maxGetter;
    public readonly Action<Vector2> minSetter, maxSetter;
    public ExposedAnchor(
        Func<Vector2> minGetter,
        Func<Vector2> maxGetter,
        Action<Vector2> minSetter,
        Action<Vector2> maxSetter)
    {
        this.minGetter = minGetter;
        this.maxGetter = maxGetter;
        this.minSetter = minSetter;
        this.maxSetter = maxSetter;
    }
}