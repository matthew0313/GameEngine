using System.IO;
using UnityEngine;

public class ImageAsset : MyAsset
{
    public readonly Sprite sprite;
    public ImageAsset(string filePath) : base(filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D texture = new(2, 2);
        texture.LoadImage(bytes);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}