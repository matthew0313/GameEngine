using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInspectable
{
    public IEnumerable<ExposedElement> GetElements();
}
public abstract class ExposedElement
{

}
public class ExposedButton : ExposedElement
{
    public readonly string name;
    public readonly Action<ExposedButton> onClick;
    public ExposedButton(string name, Action<ExposedButton> onClick)
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
    public readonly Func<ExposedVector2, Vector2> getter;
    public readonly Action<ExposedVector2, Vector2> setter;
    public ExposedVector2(string name, Func<ExposedVector2, Vector2> getter, Action<ExposedVector2, Vector2> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedFloat : ExposedProperty
{
    public readonly Func<ExposedFloat, float> getter;
    public readonly Action<ExposedFloat, float> setter;
    public ExposedFloat(string name, Func<ExposedFloat, float> getter, Action<ExposedFloat, float> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedBool : ExposedProperty
{
    public readonly Func<ExposedBool, bool> getter;
    public readonly Action<ExposedBool, bool> setter;
    public ExposedBool(string name, Func<ExposedBool, bool> getter, Action<ExposedBool, bool> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedString : ExposedProperty
{
    public readonly Func<ExposedString, string> getter;
    public readonly Action<ExposedString, string> setter;
    public ExposedString(string name, Func<ExposedString, string> getter, Action<ExposedString, string> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedAsset<T> : ExposedProperty where T : MyAsset
{
    public readonly Func<ExposedAsset<T>, T> getter;
    public readonly Action<ExposedAsset<T>, T> setter;
    public ExposedAsset(string name, Func<ExposedAsset<T>, T> getter, Action<ExposedAsset<T>, T> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}
public class ExposedObject : ExposedProperty
{
    public readonly Func<ExposedObject, MyGameObject> getter;
    public readonly Action<ExposedObject, MyGameObject> setter;
    public ExposedObject(string name, Func<ExposedObject, MyGameObject> getter, Action<ExposedObject, MyGameObject> setter) : base(name)
    {
        this.getter = getter;
        this.setter = setter;
    }
}