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
            (self) => name,
            (self, value) => name = value);
        yield return new ExposedAnchor(
            (self) => rectTransform.anchorMin,
            (self) => rectTransform.anchorMax,
            (self, value) => rectTransform.anchorMin = value,
            (self, value) => rectTransform.anchorMax = value);
        if (rectTransform.anchorMin.x == rectTransform.anchorMax.x)
        {
            yield return new ExposedFloat(
                "Pos X",
                (self) => rectTransform.anchoredPosition.x,
                (self, value) => rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y));
            yield return new ExposedFloat(
                "Width",
                (self) => rectTransform.rect.width,
                (self, value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value));
        }
        else
        {
            yield return new ExposedFloat(
                "Left",
                (self) => rectTransform.offsetMin.x,
                (self, value) => rectTransform.offsetMin = new Vector2(value, rectTransform.offsetMin.y));
            yield return new ExposedFloat(
                "Right",
                (self) => -rectTransform.offsetMax.x,
                (self, value) => rectTransform.offsetMax = new Vector2(-value, rectTransform.offsetMax.y));
        }
        if (rectTransform.anchorMin.y == rectTransform.anchorMax.y)
        {
            yield return new ExposedFloat(
                "Pos Y",
                (self) => rectTransform.anchoredPosition.y,
                (self, value) => rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, value));
            yield return new ExposedFloat(
                "Height",
                (self) => rectTransform.rect.height,
                (self, value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value));
        }
        else
        {
            yield return new ExposedFloat(
                "Top",
                (self) => -rectTransform.offsetMax.y,
                (self, value) => rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -value));
            yield return new ExposedFloat(
                "Bottom",
                (self) => rectTransform.offsetMin.y,
                (self, value) => rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, value));
        }
        yield return new ExposedVector2(
            "Pivot",
            (self) => rectTransform.pivot,
            (self, value) => rectTransform.pivot = value);
        yield return new ExposedFloat(
            "Rotation",
            (self) => transform.localEulerAngles.z,
            (self, value) => transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value));
    }
}
public class ExposedAnchor : ExposedElement
{
    readonly Func<ExposedAnchor, Vector2> minGetter, maxGetter;
    readonly Action<ExposedAnchor, Vector2> minSetter, maxSetter;
    public ExposedAnchor(
        Func<ExposedAnchor, Vector2> minGetter,
        Func<ExposedAnchor, Vector2> maxGetter,
        Action<ExposedAnchor, Vector2> minSetter,
        Action<ExposedAnchor, Vector2> maxSetter)
    {
        this.minGetter = minGetter;
        this.maxGetter = maxGetter;
        this.minSetter = minSetter;
        this.maxSetter = maxSetter;
    }
}