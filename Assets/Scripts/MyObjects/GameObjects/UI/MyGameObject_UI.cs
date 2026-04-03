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
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedFloat(
            "Width",
            (self) => rectTransform.rect.width,
            (self, value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value));
        yield return new ExposedFloat(
            "Height",
            (self) => rectTransform.rect.height,
            (self, value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value));
        yield return new ExposedVector2(
            "Pivot",
            (self) => rectTransform.pivot,
            (self, value) => rectTransform.pivot = value);
        yield return new ExposedVector2(
            "AnchorMin",
            (self) => rectTransform.anchorMin,
            (self, value) => rectTransform.anchorMin = value);
        yield return new ExposedVector2(
            "AnchorMax",
            (self) => rectTransform.anchorMax,
            (self, value) => rectTransform.anchorMax = value);
    }
}