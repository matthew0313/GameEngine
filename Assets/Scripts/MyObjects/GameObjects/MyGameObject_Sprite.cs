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
    private void OnEnable()
    {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedVector2(
            "Position",
            (self) => transform.localPosition,
            (self, value) => transform.localPosition = value);
        yield return new ExposedFloat(
            "Rotation",
            (self) => transform.localEulerAngles.z,
            (self, value) => transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value));
        yield return new ExposedList(
            "Images",
            (self) =>
            {
                List<ExposedProperty> exposedAssets = new();
                int index = 0;
                foreach (var i in images)
                {
                    ExposedAsset<ImageAsset> exposedAsset = new(
                        $"Image {index}",
                        (self) => i,
                        (self, value) =>
                        {
                            if(self.listIndex < 0 || self.listIndex >= images.Count)
                            {
                                EditorSceneManager.Instance.AddLog(new MyLog(
                                    MyLogType.Error, $"Invalid image index in gameObject {name}"
                                ));
                                return;
                            }
                            images[self.listIndex] = value;
                        }
                    )
                    {
                        listIndex = index++
                    };
                    exposedAssets.Add(exposedAsset);
                }
                return exposedAssets;
            },
            (index) =>
            {
                if (index < 0 || index > images.Count) return;
                images.Insert(index, null);
            },
            (index) =>
            {
                if (index < 0 || index >= images.Count) return;
                images.RemoveAt(index);
            },
            (index1, index2) =>
            {
                if (index1 < 0 || index1 >= images.Count || index2 < 0 || index2 >= images.Count) return;
                var temp = images[index1];
                images[index1] = images[index2];
                images[index2] = temp;
            });
        yield return new ExposedFloat(
            "Image Index",
            (self) => imageIndex,
            (self, value) => SetImageIndex(Mathf.FloorToInt(value))
        );
        yield return new ExposedFloat(
            "Order in Layer",
            (self) => spriteRenderer.sortingOrder,
            (self, value) => spriteRenderer.sortingOrder = Mathf.FloorToInt(value));
    }
    public void SetImageIndex(int index)
    {
        if(index < 0 || index >= images.Count)
        {
            spriteRenderer.sprite = null;
            imageIndex = -1;
            EditorSceneManager.Instance.AddLog(new MyLog(
                MyLogType.Warning, $"Invalid image index {index} for Sprite GameObject {name}"
            ));
            return;
        }
        ImageAsset imageAsset = images[index];
        if (imageAsset == null)
        {
            spriteRenderer.sprite = null;
            imageIndex = -1;
            EditorSceneManager.Instance.AddLog(new MyLog(
                MyLogType.Warning, $"Image asset not found for path {images[index]} in Sprite GameObject {name}"
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