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
    public abstract float GetWidth();
}
public class MyArray
{
    public List<MyArrayElement> elements = new();
}
public class MyArrayElement
{
    public PropertyType type;
    public float number = 0;
    public bool condition = false;
    public string str = "";
    public MyGameObject obj = null;
    public MyAsset asset = null;
    public MyArray array = null;
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
    Array = 1<<5,
    Wildcard = 1<<0 + 1<<1 + 1<<2 + 1<<3 + 1<<4 + 1<<5
}