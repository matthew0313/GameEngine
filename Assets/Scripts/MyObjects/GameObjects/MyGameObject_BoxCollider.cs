using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MyGameObject_BoxCollider : MyGameObject
{
    new BoxCollider2D collider;
    public override string id => "BoxCollider";
    protected override void Awake()
    {
        base.Awake();
        collider = GetComponent<BoxCollider2D>();
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedBool(
            "IsTrigger",
            () => collider.isTrigger,
            (value) => collider.isTrigger = value);
    }
}