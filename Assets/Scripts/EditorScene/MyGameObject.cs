using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MyGameObject : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public readonly List<string> images = new();
    public bool dirty = false;
    public event Action onStart;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void MyStart()
    {
        onStart?.Invoke();
    }
    public void SetImageIndex(int index)
    {
        if(index < 0 || index >= images.Count)
        {
            spriteRenderer.sprite = null;
            EditorSceneManager.Instance.AddLog(new MyLog(
                MyLogType.Warning, $"Invalid image index {index} for GameObject {name}"
            ));
            return;
        }
        
        ImageAsset imageAsset = EditorSceneManager.Instance.GetAsset<ImageAsset>(images[index]);
        if(imageAsset == null)
        {
            spriteRenderer.sprite = null;
            EditorSceneManager.Instance.AddLog(new MyLog(
                MyLogType.Warning, $"Image asset not found for path {images[index]} in GameObject {name}"
            ));
            return;
        }

        spriteRenderer.sprite = imageAsset.sprite;
    }
    public MyGameObjectSave Save()
    {
        MyGameObjectSave save = new();
        save.position = transform.position;
        save.images = images;
        return save;
    }
    public void Load(MyGameObjectSave save)
    {
        transform.position = save.position;
        images.Clear();
        images.AddRange(save.images);
    }
}
[System.Serializable]
public class MyGameObjectSave
{
    public Vector2 position;
    public List<string> images = new();
}