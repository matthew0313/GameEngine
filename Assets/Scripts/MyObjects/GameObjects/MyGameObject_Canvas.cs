using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MyGameObject_Canvas : MyGameObject
{
    Canvas canvas;
    RectTransform rectTransform;
    public override string id => "Canvas";
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
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.floats["width"] = rectTransform.rect.width;
        save.data.floats["height"] = rectTransform.rect.height;
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, save.data.floats["width"]);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, save.data.floats["height"]);
    }
}