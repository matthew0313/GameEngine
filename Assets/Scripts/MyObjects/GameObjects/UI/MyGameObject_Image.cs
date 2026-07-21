using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MyGameObject_Image : MyGameObject_UI
{
    public override string id => "Image";
    Image imageComp;

    public ImageAsset image { get; private set; }
    Sprite defaultSprite;
    private void OnEnable()
    {
        imageComp ??= GetComponent<Image>();
        defaultSprite ??= imageComp.sprite;
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedAsset(
            "Image",
            () => image,
            (value) => SetImage(value as ImageAsset),
            AssetType.Image);
        yield return new ExposedColor(
            "Color",
            () => imageComp.color,
            (value) => imageComp.color = value);
    }
    public void SetImage(ImageAsset image)
    {
        if(this.image != null) this.image.onSpriteChange -= OnSpriteChange;
        this.image = image;
        if (image != null)
        {
            image.onSpriteChange += OnSpriteChange;
            OnSpriteChange(image.sprite);
        }
        else OnSpriteChange(null);
    }

    private void OnSpriteChange(Sprite sprite)
    {
        imageComp.sprite = sprite == null ? defaultSprite : sprite;
    }

    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.ulongs["image"] = image != null ? image.uid : 0;
        save.data.SaveColor("color", imageComp.color);
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        if (save.data.ulongs.TryGetValue("image", out ulong imageId))
            SetImage(EditorSceneManager.Instance.GetAsset<ImageAsset>(imageId));
        if (save.data.TryLoadColor("color", out Color color)) imageComp.color = color;
    }
}