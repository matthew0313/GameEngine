using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class MyGameObject_UI : MyGameObject
{
    public RectTransform rectTransform { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
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
            (value) =>
            {
                Rect tmp = rectTransform.rect;
                rectTransform.anchorMin = value;
                rectTransform.rect.Set(tmp.x, tmp.y, tmp.width, tmp.height);
                OnInspectorChange();
            },
            (value) =>
            {
                Rect tmp = rectTransform.rect;
                rectTransform.anchorMax = value;
                rectTransform.rect.Set(tmp.x, tmp.y, tmp.width, tmp.height);
                OnInspectorChange();
            });
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
        yield return new ExposedVector2(
            "Scale",
            () => transform.localScale,
            (value) => transform.localScale = new Vector3(value.x, value.y, 1.0f));
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.SaveVector2("anchorMin", rectTransform.anchorMin);
        save.data.SaveVector2("anchorMax", rectTransform.anchorMax);
        save.data.SaveVector2("anchoredPosition", rectTransform.anchoredPosition);
        save.data.SaveVector2("offsetMin", rectTransform.offsetMin);
        save.data.SaveVector2("offsetMax", rectTransform.offsetMax);
        save.data.SaveVector2("pivot", rectTransform.pivot);
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        rectTransform.anchorMin = save.data.LoadVector2("anchorMin");
        rectTransform.anchorMax = save.data.LoadVector2("anchorMax");
        rectTransform.anchoredPosition = save.data.LoadVector2("anchoredPosition");
        rectTransform.offsetMin = save.data.LoadVector2("offsetMin");
        rectTransform.offsetMax = save.data.LoadVector2("offsetMax");
        rectTransform.pivot = save.data.LoadVector2("pivot");
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