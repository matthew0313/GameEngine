using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MyGameObject_Canvas : MyGameObject
{
    public Canvas canvas { get; private set; }
    RectTransform rectTransform;
    public override string id => "Canvas";
    private void OnEnable()
    {
        canvas ??= GetComponent<Canvas>();
        rectTransform ??= canvas.GetComponent<RectTransform>();
    }
    private void Update()
    {
        canvas.sortingOrder = Mathf.Clamp(canvas.sortingOrder, -10000, 10000);
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
        yield return new ExposedNumber(
            "Order in Layer",
            () => canvas.sortingOrder,
            (value) => canvas.sortingOrder = Mathf.FloorToInt(value));
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.floats["width"] = rectTransform.rect.width;
        save.data.floats["height"] = rectTransform.rect.height;
        save.data.integers["orderInLayer"] = canvas.sortingOrder;
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        if (save.data.floats.TryGetValue("width", out float width)) rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        if (save.data.floats.TryGetValue("height", out float height)) rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        if (save.data.integers.TryGetValue("orderInLayer", out int orderInLayer)) canvas.sortingOrder = orderInLayer;
    }
}