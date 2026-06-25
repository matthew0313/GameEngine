using System.Collections.Generic;
using UnityEngine;

public abstract class PropertyCodeBlock : CodeBlock
{
    public abstract PropertyType propertyType { get; }
    public virtual float GetNumber(ulong hash) => 0;
    public virtual bool GetCondition(ulong hash) => false;
    public virtual string GetString(ulong hash) => "";
    public virtual MyGameObject GetObject(ulong hash) => null;
    public virtual MyAsset GetAsset(ulong hash) => null;
    public virtual Vector2 GetVector2(ulong hash) => new();
    public virtual Color GetColor(ulong hash) => Color.white;
    public virtual List<Wildcard> GetArray(ulong hash) => null;
    public Wildcard GetWildcard(ulong hash) => new()
    {
        number = GetNumber(hash),
        condition = GetCondition(hash),
        str = GetString(hash),
        obj = GetObject(hash),
        asset = GetAsset(hash),
        vector2 = GetVector2(hash),
        color = GetColor(hash),
        array = GetArray(hash)
    };
    public abstract float GetWidth();
}
[System.Serializable]
[System.Flags]
public enum PropertyType
{
    Number = 1<<0,
    Condition = 1<<1,
    String = 1<<2,
    Object = 1<<3,
    Asset = 1<<4,
    Vector2 = 1<<5,
    Array = 1<<6,
    Color = 1<<7,
    Wildcard = Number + Condition + String + Object + Asset + Vector2 + Array + Color
}
[System.Serializable]
public struct Wildcard
{
    public float number;
    public bool condition;
    public string str;
    public MyGameObject obj;
    public MyAsset asset;
    public Vector2 vector2;
    public Color color;
    public List<Wildcard> array;
}