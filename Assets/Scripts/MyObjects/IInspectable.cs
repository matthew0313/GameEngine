using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInspectable
{
    public IEnumerable<ExposedElement> GetElements();
    public Action onInspectorChange { get; set; }
}
public abstract class ExposedElement
{
    public bool visible = true;
}
public class ExposedButton : ExposedElement
{
    public readonly string name;
    public readonly Action onClick;
    public ExposedButton(string name, Action onClick)
    {
        this.name = name;
        this.onClick = onClick;
    }
}
public abstract class ExposedProperty : ExposedElement
{
    public readonly string name;
    public ExposedProperty(string name)
    {
        this.name = name;
    }
}
public class ExposedVector2 : ExposedProperty
{
    public readonly Func<Vector2> getter;
    public readonly Action<Vector2> setter;
    public ExposedVector2(string name, Func<Vector2> getter, Action<Vector2> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedNumber : ExposedProperty
{
    public readonly Func<float> getter;
    public readonly Action<float> setter;
    public ExposedNumber(string name, Func<float> getter, Action<float> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedDropdown : ExposedProperty
{
    public readonly Func<int> getter;
    public readonly Action<int> setter;
    public readonly string[] options;
    public ExposedDropdown(string name, Func<int> getter, Action<int> setter, string[] options) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
        this.options = options;
    }
}
public class ExposedBool : ExposedProperty
{
    public readonly Func<bool> getter;
    public readonly Action<bool> setter;
    public ExposedBool(string name, Func<bool> getter, Action<bool> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedString : ExposedProperty
{
    public readonly Func<string> getter;
    public readonly Action<string> setter;
    public ExposedString(string name, Func<string> getter, Action<string> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedAsset : ExposedProperty
{
    public readonly Func<MyAsset> getter;
    public readonly Action<MyAsset> setter;
    public readonly AssetType assetType;
    public ExposedAsset(string name, Func<MyAsset> getter, Action<MyAsset> setter, AssetType assetType) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
        this.assetType = assetType;
    }
}
public class ExposedObject : ExposedProperty
{
    public readonly Func<MyGameObject> getter;
    public readonly Action<MyGameObject> setter;
    public bool IDSpecific = false;
    public string id;
    public ExposedObject(string name, Func<MyGameObject> getter, Action<MyGameObject> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}