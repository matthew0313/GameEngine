using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class ImageAsset : FileAsset
{
    public override AssetType type => AssetType.Image;
    public Sprite sprite { get; private set; }
    public event Action<Sprite> onSpriteChange;

    Vector2 pivot = new Vector2(0.5f, 0.5f);
    public override void LoadFile(string filePath)
    {
        if(!File.Exists(filePath) || !filePath.EndsWith(".png"))
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Warning,
                message = $"Failed to load image asset: {filePath}"
            });
            sprite = null;
            onSpriteChange?.Invoke(null);
            return;
        }
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D texture = new(2, 2);
        texture.LoadImage(bytes);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot);
        onSpriteChange?.Invoke(sprite);
        this.filePath = filePath;
        OnDisplayUpdate();
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach(var i in base.GetElements()) yield return i;
        yield return new ExposedVector2(
            "Pivot",
            () => pivot,
            (value) => 
            { 
                pivot = value; 
                if (sprite != null)
                {
                    sprite = Sprite.Create(sprite.texture, new Rect(sprite.rect), pivot);
                    onSpriteChange?.Invoke(sprite);
                }
            });
    }
    public override MyAssetSave Save()
    {
        var save = base.Save();
        save.data.floats["pivotX"] = pivot.x;
        save.data.floats["pivotY"] = pivot.y;
        return save;
    }
    public override void Load(MyAssetSave save)
    {
        base.Load(save);
        pivot = new Vector2(save.data.floats["pivotX"], save.data.floats["pivotY"]);
    }
}