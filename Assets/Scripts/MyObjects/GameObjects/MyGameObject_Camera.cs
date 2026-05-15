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
            () => transform.localPosition,
            (value) => transform.localPosition = value);
        yield return new ExposedFloat(
            "Rotation",
            () => transform.localEulerAngles.z,
            (value) => transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value));
        yield return new ExposedFloat(
            "OrthographicSize",
            () => cam.orthographicSize,
            (value) => cam.orthographicSize = value);
        yield return new ExposedVector2(
            "Size",
            () => size,
            (value) =>
            {
                size = new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
                renderTexture = new RenderTexture(size.x, size.y, 24);
                cam.targetTexture = renderTexture;
            });
    }
}
