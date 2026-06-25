using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MyGameObject_Sprite : MyGameObject
{
    public override string id => "Sprite";
    public SpriteRenderer spriteRenderer { get; private set; }

    public ImageAsset image { get; private set; }
    Sprite defaultSprite;
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }
    private void Update()
    {
        spriteRenderer.sortingOrder = Mathf.Clamp(spriteRenderer.sortingOrder, -10000, 10000);
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
            () => spriteRenderer.color,
            (value) => spriteRenderer.color = value);
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
        if (save.data.ulongs.TryGetValue("image", out ulong imageId))
            SetImage(EditorSceneManager.Instance.GetAsset<ImageAsset>(imageId));
        if (save.data.integers.TryGetValue("orderInLayer", out int orderInLayer)) spriteRenderer.sortingOrder = orderInLayer;
    }
}