using System;
using System.Collections.Generic;
using UnityEngine;

public class MyGameObject_Camera : MyGameObject
{
    public override MyGameObjectType type => MyGameObjectType.Camera;
    [field:SerializeField] public Camera cam { get; private set; }
    public RenderTexture renderTexture { get; private set; }
    public event Action<RenderTexture> onTextureChange;

    Vector2Int m_size = new Vector2Int(600, 600);
    public Vector2 size
    {
        get => m_size;
        set
        {
            m_size = new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
            renderTexture = new RenderTexture(m_size.x, m_size.y, 24);
            cam.targetTexture = renderTexture;
            onTextureChange?.Invoke(renderTexture);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        size = m_size;
    }

    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedNumber(
            "Orthographic Size",
            () => cam.orthographicSize,
            (value) => cam.orthographicSize = value);
        yield return new ExposedVector2(
            "Size",
            () => size,
            (value) => size = value);
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.floats["orthographicSize"] = cam.orthographicSize;
        save.data.SaveVector2("size", size);
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        cam.orthographicSize = save.data.floats["orthographicSize"];
        size = save.data.LoadVector2("size");
    }
}
