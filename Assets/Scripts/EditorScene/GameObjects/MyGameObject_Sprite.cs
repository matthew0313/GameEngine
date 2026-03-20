using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MyGameObject_Sprite : MyGameObject
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public override string id => "Sprite";
    public readonly List<string> images = new();
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetImageIndex(int index)
    {
        if(index < 0 || index >= images.Count)
        {
            spriteRenderer.sprite = null;
            EditorSceneManager.Instance.AddLog(new MyLog(
                MyLogType.Warning, $"Invalid image index {index} for Sprite GameObject {name}"
            ));
            return;
        }
        
        ImageAsset imageAsset = EditorSceneManager.Instance.GetAsset<ImageAsset>(images[index]);
        if(imageAsset == null)
        {
            spriteRenderer.sprite = null;
            EditorSceneManager.Instance.AddLog(new MyLog(
                MyLogType.Warning, $"Image asset not found for path {images[index]} in Sprite GameObject {name}"
            ));
            return;
        }

        spriteRenderer.sprite = imageAsset.sprite;
    }
}