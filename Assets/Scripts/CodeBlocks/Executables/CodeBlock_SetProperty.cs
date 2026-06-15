using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Codeblock_SetProperty : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Dropdown propertyDropdown;
    [SerializeField] WildcardSnapPoint value;
    [field: SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override bool IsAddable(ICodeable codeable)
    {
        if (base.IsAddable(codeable) && codeable is IInspectable inspectable)
        {
            foreach(var i in inspectable.GetElements())
            {
                if(i is ExposedProperty || i is ExposedAnchor)
                {
                    return true;
                }
            }
        }
        return false;
    }
    readonly List<Action<WildcardSnapPoint, ulong>> cache = new();
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        cache.Clear();
        IInspectable inspectable = owner as IInspectable;
        propertyDropdown.ClearOptions();
        foreach(var i in inspectable.GetElements())
        {
            if(i is ExposedProperty property)
            {
                if(i is ExposedVector2 vector2)
                {
                    propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name + ".x"));
                    cache.Add((snap, hash) => vector2.setter(new Vector2(snap.GetNumber(hash), vector2.getter().y)));
                    propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name + ".y"));
                    cache.Add((snap, hash) => vector2.setter(new Vector2(vector2.getter().x, snap.GetNumber(hash))));
                }
                else
                {
                    propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name));
                    if (i is ExposedNumber number) cache.Add((snap, hash) => number.setter(snap.GetNumber(hash)));
                    else if (i is ExposedBool boolean) cache.Add((snap, hash) => boolean.setter(snap.GetCondition(hash)));
                    else if (i is ExposedString str) cache.Add((snap, hash) => str.setter(snap.GetString(hash)));
                    else if (i is ExposedDropdown dropdown) cache.Add((snap, hash) => dropdown.setter((int)snap.GetNumber(hash)));
                    else if (i is ExposedObject obj) cache.Add((snap, hash) => obj.setter(snap.GetObject(hash)));
                    else if (i is ExposedAsset asset) cache.Add((snap, hash) => asset.setter(snap.GetAsset(hash)));
                }
            }
            else if(i is ExposedAnchor anchor)
            {
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMin.x"));
                cache.Add((snap, hash) => { Vector2 min = anchor.minGetter(); anchor.minSetter(new Vector2(snap.GetNumber(hash), min.y)); });
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMin.y"));
                cache.Add((snap, hash) => { Vector2 min = anchor.minGetter(); anchor.minSetter(new Vector2(min.x, snap.GetNumber(hash))); });
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMax.x"));
                cache.Add((snap, hash) => { Vector2 max = anchor.maxGetter(); anchor.maxSetter(new Vector2(snap.GetNumber(hash), max.y)); });
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMax.y"));
                cache.Add((snap, hash) => { Vector2 max = anchor.maxGetter(); anchor.maxSetter(new Vector2(max.x, snap.GetNumber(hash))); });
            }
        }
        propertyDropdown.RefreshShownValue();
    }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        cache[propertyDropdown.value].Invoke(value, hash);
        (owner as IInspectable).onInspectorChange?.Invoke();
        return await onFinish.Execute(hash);
    }
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.integers["property"] = propertyDropdown.value;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        propertyDropdown.value = save.data.integers["property"];
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return value;
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}