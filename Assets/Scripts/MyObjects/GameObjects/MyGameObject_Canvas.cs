using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MyGameObject_Canvas : MyGameObject
{
    Canvas canvas;
    RectTransform rectTransform;
    public override MyGameObjectType type => throw new NotImplementedException();
    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        rectTransform = canvas.GetComponent<RectTransform>();
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedNumber(
            "Width",
            () => rectTransform.rect.width,
            (value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value));
        yield return new ExposedNumber(
            "Height",
            () => rectTransform.rect.height,
            (value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value));
    }
}