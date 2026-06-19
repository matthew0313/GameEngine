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
                onInspectorChange?.Invoke();
            },
            (value) =>
            {
                Rect tmp = rectTransform.rect;
                rectTransform.anchorMax = value;
                rectTransform.rect.Set(tmp.x, tmp.y, tmp.width, tmp.height);
                onInspectorChange?.Invoke();
            });
        bool xPointAnchor = rectTransform.anchorMin.x == rectTransform.anchorMax.x;
        bool yPointAnchor = rectTransform.anchorMin.y == rectTransform.anchorMax.y;
        yield return new ExposedNumber(
            "Pos X",
            () => rectTransform.anchoredPosition.x,
            (value) => rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y)) { visible = xPointAnchor };
        yield return new ExposedNumber(
            "Width",
            () => rectTransform.rect.width,
            (value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value)) { visible = xPointAnchor };
        yield return new ExposedNumber(
            "Left",
            () => rectTransform.offsetMin.x,
            (value) => rectTransform.offsetMin = new Vector2(value, rectTransform.offsetMin.y)) { visible = !xPointAnchor };
        yield return new ExposedNumber(
            "Right",
            () => -rectTransform.offsetMax.x,
            (value) => rectTransform.offsetMax = new Vector2(-value, rectTransform.offsetMax.y)) { visible = !xPointAnchor };
        yield return new ExposedNumber(
            "Pos Y",
            () => rectTransform.anchoredPosition.y,
            (value) => rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, value)) { visible = yPointAnchor };
        yield return new ExposedNumber(
            "Height",
            () => rectTransform.rect.height,
            (value) => rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value)) { visible = yPointAnchor };
        yield return new ExposedNumber(
            "Top",
            () => -rectTransform.offsetMax.y,
            (value) => rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -value)) { visible = !yPointAnchor };
        yield return new ExposedNumber(
            "Bottom",
            () => rectTransform.offsetMin.y,
            (value) => rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, value)) { visible = !yPointAnchor };
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
        save.data.SaveVector2("offsetMin", rectTransform.offsetMin);
        save.data.SaveVector2("offsetMax", rectTransform.offsetMax);
        save.data.SaveVector2("pivot", rectTransform.pivot);
        save.data.SaveVector2("anchoredPosition", rectTransform.anchoredPosition);
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        if (save.data.TryLoadVector2("anchorMin", out Vector2 anchorMin)) rectTransform.anchorMin = anchorMin;
        if (save.data.TryLoadVector2("anchorMax", out Vector2 anchorMax)) rectTransform.anchorMax = anchorMax;
        if (save.data.TryLoadVector2("offsetMin", out Vector2 offsetMin)) rectTransform.offsetMin = offsetMin;
        if (save.data.TryLoadVector2("offsetMax", out Vector2 offsetMax)) rectTransform.offsetMax = offsetMax;
        if (save.data.TryLoadVector2("pivot", out Vector2 pivot)) rectTransform.pivot = pivot;
        if (save.data.TryLoadVector2("anchoredPosition", out Vector2 anchoredPosition)) rectTransform.anchoredPosition = anchoredPosition;
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