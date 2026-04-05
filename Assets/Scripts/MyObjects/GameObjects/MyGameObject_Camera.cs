using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MyGameObject_Camera : MyGameObject
{
    public Camera cam { get; private set; }
    public RenderTexture renderTexture { get; private set; }

    public float priority = 0;
    public override string id => "Camera";

    public Vector2Int size { get; private set; } = new Vector2Int(600, 600);
    protected override void Awake()
    {
        base.Awake();
        cam = GetComponent<Camera>();
        renderTexture = new RenderTexture(size.x, size.y, 24);
        cam.targetTexture = renderTexture;
    }

    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedVector2(
            "Position",
            (self) => transform.localPosition,
            (self, value) => transform.localPosition = value);
        yield return new ExposedFloat(
            "Rotation",
            (self) => transform.localEulerAngles.z,
            (self, value) => transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value));
        yield return new ExposedFloat(
            "OrthographicSize",
            (self) => cam.orthographicSize,
            (self, value) => cam.orthographicSize = value);
        yield return new ExposedFloat(
            "Priority",
            (self) => priority,
            (self, value) => priority = value);
    }
}
