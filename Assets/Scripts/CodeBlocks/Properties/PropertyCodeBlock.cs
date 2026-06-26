using System.Collections.Generic;
using UnityEngine;

public abstract class PropertyCodeBlock : CodeBlock
{
    public abstract PropertyType propertyType { get; }
    public virtual float GetNumber(ulong hash) => Wildcard.Default().number;
    public virtual bool GetCondition(ulong hash) => Wildcard.Default().condition;
    public virtual string GetString(ulong hash) => Wildcard.Default().str;
    public virtual MyGameObject GetObject(ulong hash) => Wildcard.Default().obj;
    public virtual MyAsset GetAsset(ulong hash) => Wildcard.Default().asset;
    public virtual Vector2 GetVector2(ulong hash) => Wildcard.Default().vector2;
    public virtual Color GetColor(ulong hash) => Wildcard.Default().color;
    public virtual ulong GetHash(ulong hash) => Wildcard.Default().hash;
    public virtual List<Wildcard> GetArray(ulong hash) => Wildcard.Default().array;
    public Wildcard GetWildcard(ulong hash) => new()
    {
        number = GetNumber(hash),
        condition = GetCondition(hash),
        str = GetString(hash),
        obj = GetObject(hash),
        asset = GetAsset(hash),
        vector2 = GetVector2(hash),
        color = GetColor(hash),
        hash = GetHash(hash),
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
    Hash = 1<<8,
    Wildcard = Number + Condition + String + Object + Asset + Vector2 + Array + Color + Hash
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
    public ulong hash;
    public List<Wildcard> array;
    public static Wildcard Default()
    {
        Wildcard tmp = new();
        tmp.number = 0;
        tmp.condition = false;
        tmp.str = null;
        tmp.obj = null;
        tmp.asset = null;
        tmp.vector2 = Vector2.zero;
        tmp.color = Color.white;
        tmp.hash = ulong.MaxValue;
        tmp.array = null;
        return tmp;
    }
}