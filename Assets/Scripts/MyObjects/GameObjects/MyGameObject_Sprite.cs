using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MyGameObject_Sprite : MyGameObject
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public override string id => "Sprite";
    public int imageIndex { get; private set; } = -1;
    public readonly List<ImageAsset> images = new();
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
        yield return new ExposedNumber(
            "Image Index",
            () => imageIndex,
            (value) => SetImageIndex(Mathf.FloorToInt(value))
        );
        yield return new ExposedNumber(
            "Order in Layer",
            () => spriteRenderer.sortingOrder,
            (value) => spriteRenderer.sortingOrder = Mathf.FloorToInt(value));
    }
    public void SetImageIndex(int index)
    {
        if(index < 0 || index >= images.Count)
        {
            spriteRenderer.sprite = defaultSprite;
            imageIndex = -1;
            EditorSceneManager.Instance.AddLog(new MyLog(
                MyLogType.Warning, $"Invalid image index {index} for Sprite GameObject {name}"
            ));
            return;
        }
        ImageAsset imageAsset = images[index];
        if (imageAsset == null)
        {
            spriteRenderer.sprite = defaultSprite;
            EditorSceneManager.Instance.AddLog(new MyLog(
                MyLogType.Warning, $"Image asset is null in index {index} for Sprite GameObject {name}"
            ));
            return;
        }

        spriteRenderer.sprite = imageAsset.sprite;
        imageIndex = index;
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        List<ulong> images = new();
        foreach(var i in this.images)
        {
            images.Add(i.uid);
        }
        save.data.strings["images"] = JsonUtility.ToJson(images, prettyPrint);
        save.data.integers["imageIndex"] = imageIndex;
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        if(save.data.strings.ContainsKey("images"))
        {
            List<ulong> images = JsonUtility.FromJson<List<ulong>>(save.data.strings["images"]);
            images.Clear();
            foreach(var i in images)
            {
                ImageAsset imageAsset = EditorSceneManager.Instance.GetAsset<ImageAsset>(i);
                if(imageAsset == null)
                {
                    EditorSceneManager.Instance.AddLog(new MyLog(
                        MyLogType.Warning, $"Image asset not found for path {i} in Sprite GameObject {name}"
                    ));
                    continue;
                }
                this.images.Add(imageAsset);
            }
        }
        if (save.data.integers.TryGetValue("imageIndex", out int imageIndex)) SetImageIndex(imageIndex);
    }
}