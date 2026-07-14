using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class MyGameObject_Screen : MyGameObject_UI
{
    public override string id => "Screen";
    public RawImage rawImage { get; private set; }
    MyGameObject_Camera boundCamera;
    private void OnEnable()
    {
        rawImage ??= GetComponent<RawImage>();
    }
    void SetCamera(MyGameObject_Camera cam)
    {
        if (boundCamera != null) boundCamera.onTextureChange -= OnTextureChange;
        boundCamera = cam;
        if (boundCamera != null)
        {
            boundCamera.onTextureChange += OnTextureChange;
            OnTextureChange(boundCamera.renderTexture);
        }
        else OnTextureChange(null);
    }
    void OnTextureChange(RenderTexture texture)
    {
        rawImage.texture = texture;
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedObject(
            "Bound Camera",
            () => boundCamera,
            (value) =>
            {
                if (value is MyGameObject_Camera cam) SetCamera(cam);
            }) { IDSpecific = true, id = "Camera" };
        yield return new ExposedColor(
            "Color",
            () => rawImage.color,
            (value) => rawImage.color = value);
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.ulongs["boundCamera"] = boundCamera != null ? boundCamera.uid : 0;
        save.data.SaveColor("color", rawImage.color);
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        if (save.data.ulongs.TryGetValue("boundCamera", out ulong boundCameraId))
            SetCamera(EditorSceneManager.Instance.FindObjectWithUID(boundCameraId) as MyGameObject_Camera);
        if (save.data.TryLoadColor("color", out Color color)) rawImage.color = color;
    }
}