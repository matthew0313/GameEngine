using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MyGameObject_Image : MyGameObject_UI
{
    public Image imageComp { get; private set; }
    public override string id => "Image";

    public ImageAsset image { get; private set; }
    Sprite defaultSprite;
    protected override void Awake()
    {
        base.Awake();
        imageComp = GetComponent<Image>();
        defaultSprite = imageComp.sprite;
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedAsset(
            "Image",
            () => image,
            (value) => SetImage(value as ImageAsset),
            AssetType.Image);
    }
    public void SetImage(ImageAsset image)
    {
        this.image = image;
        if (image == null)
        {
            imageComp.sprite = defaultSprite;
            return;
        }
        imageComp.sprite = image.sprite;
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.ulongs["image"] = image != null ? image.uid : 0;
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        SetImage(EditorSceneManager.Instance.GetAsset<ImageAsset>(save.data.ulongs["image"]));
    }
}