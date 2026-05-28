using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MyGameObject_Sprite : MyGameObject
{
    public override MyGameObjectType type => MyGameObjectType.Sprite;
    public SpriteRenderer spriteRenderer { get; private set; }

    public ImageAsset image { get; private set; }
    Sprite defaultSprite;
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedAsset(
            "Image",
            () => image,
            (value) => SetImage(value as ImageAsset),
            AssetType.Image);
        yield return new ExposedNumber(
            "Order in Layer",
            () => spriteRenderer.sortingOrder,
            (value) => spriteRenderer.sortingOrder = Mathf.FloorToInt(value));
    }
    public void SetImage(ImageAsset image)
    {
        if(this.image != null)
        {
            this.image.onSpriteChange -= OnSpriteChange;
        }
        this.image = image;
        if (image == null)
        {
            spriteRenderer.sprite = defaultSprite;
            return;
        }
        else image.onSpriteChange += OnSpriteChange;
        OnSpriteChange(image.sprite);
    }
    void OnSpriteChange(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.ulongs["image"] = image != null ? image.uid : 0;
        save.data.integers["orderInLayer"] = spriteRenderer.sortingOrder;
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        SetImage(EditorSceneManager.Instance.GetAsset<ImageAsset>(save.data.ulongs["image"]));
        spriteRenderer.sortingOrder = save.data.integers["orderInLayer"];
    }
}