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

    }
}