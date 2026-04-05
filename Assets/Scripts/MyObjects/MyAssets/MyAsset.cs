using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyAsset : ISelectable, IInspectable
{
    public ulong uid { get; private set; } = MathUtilities.GenerateRandomID();
    public abstract AssetType type { get; }

    public string name;
    public event Action onUpdate;
    public virtual IEnumerable<ExposedElement> GetElements()
    {
        yield return new ExposedString(
            "Name",
            (self) => name,
            (self, value) => { name = value; onUpdate?.Invoke(); });
    }
    protected virtual void OnUpdate() => onUpdate?.Invoke();
    public virtual MyAssetSave Save()
    {
        MyAssetSave save = new();
        save.type = type;
        save.name = name;
        save.uid = uid;
        return save;
    }
    public virtual void EarlyLoad(MyAssetSave save)
    {
        uid = save.uid;
    }
    public virtual void Load(MyAssetSave save)
    {
        name = save.name;
    }

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }
}
[System.Serializable]
public class MyAssetSave
{
    public AssetType type;
    public string name;
    public ulong uid;
    public DataUnit data = new();
}
[System.Serializable]
[Flags]
public enum AssetType
{
    Image = 1 << 0,
    Prefab = 1 << 1
}
