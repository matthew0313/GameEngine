using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class ImageAsset : MyAsset, IFileAsset
{
    public string filePath { get; private set; }
    public override AssetType type => AssetType.Image;
    public Sprite sprite { get; private set; }

    Vector2 pivot = new Vector2(0.5f, 0.5f);
    public void LoadFile(string filePath)
    {
        if(!Directory.Exists(filePath) || !filePath.EndsWith(".png"))
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Warning,
                message = $"Failed to load image asset: {filePath}"
            });
        }
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D texture = new(2, 2);
        texture.LoadImage(bytes);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot);
        this.filePath = filePath;
        OnUpdate();
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach(var i in base.GetElements()) yield return i;
        yield return new ExposedString(
            "FilePath",
            (self) => filePath,
            (self, value) => LoadFile(value));
        yield return new ExposedVector2(
            "Pivot",
            (self) => pivot,
            (self, value) => { pivot = value; if(sprite != null) sprite = Sprite.Create(sprite.texture, new Rect(sprite.rect), pivot); });
    }
    public override MyAssetSave Save()
    {
        var save = base.Save();
        save.data.strings["filePath"] = filePath;
        save.data.floats["pivotX"] = pivot.x;
        save.data.floats["pivotY"] = pivot.y;
        return save;
    }
    public override void Load(MyAssetSave save)
    {
        base.Load(save);
        pivot = new Vector2(save.data.floats["pivotX"], save.data.floats["pivotY"]);
        LoadFile(save.data.strings["filePath"]);
    }
}