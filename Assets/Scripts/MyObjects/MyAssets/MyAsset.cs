using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyAsset : ISelectable, IInspectable
{
    public ulong uid { get; private set; } = MathUtilities.GenerateRandomID();
    public abstract AssetType type { get; }
    public abstract Sprite assetImage { get; }

    public string name;
    public event Action onDisplayUpdate;
    public Action onInspectorChange { get; set; }
    public virtual IEnumerable<ExposedElement> GetElements()
    {
        yield return new ExposedString(
            "Name",
            () => name,
            (value) => { name = value; OnDisplayUpdate(); });
    }
    protected virtual void OnDisplayUpdate() => onDisplayUpdate?.Invoke();
    public virtual MyAssetSave Save()
    {
        MyAssetSave save = new();
        save.type = type;
        save.name = name;
        save.uid = uid;
        return save;
    }
    public virtual void EarlyLoad(MyAssetSave save, bool resetUID = false)
    {
        uid = resetUID ? MathUtilities.GenerateRandomID() : save.uid;
    }
    public virtual void Load(MyAssetSave save)
    {
        name = save.name;
    }
    public static MyAsset TypeToAsset(AssetType type)
    {
        return type switch
        {
            AssetType.Image => new ImageAsset(),
            AssetType.Prefab => new PrefabAsset(),
            AssetType.Scene => new SceneAsset(),
            _ => null
        };
    }
    public event Action onRemove;
    public virtual void OnRemove()
    {
        onRemove?.Invoke();
    }
    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }
}
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
    Prefab = 1 << 1,
    Scene = 1 << 2
}
