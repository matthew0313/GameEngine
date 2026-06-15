using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Codeblock_GetProperty : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Wildcard;
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Dropdown propertyDropdown;
    public override bool IsAddable(ICodeable codeable)
    {
        if (base.IsAddable(codeable) && codeable is IInspectable inspectable)
        {
            foreach (var i in inspectable.GetElements())
            {
                if (i is ExposedProperty || i is ExposedAnchor)
                {
                    return true;
                }
            }
        }
        return false;
    }
    readonly List<Func<ulong, Wildcard>> cache = new();
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        cache.Clear();
        IInspectable inspectable = owner as IInspectable;
        propertyDropdown.ClearOptions();
        foreach (var i in inspectable.GetElements())
        {
            if (i is ExposedProperty property)
            {
                if (i is ExposedVector2 vector2)
                {
                    propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name + ".x"));
                    cache.Add(_ => new()
                    {
                        number = vector2.getter().x
                    });
                    propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name + ".y"));
                    cache.Add(_ => new()
                    {
                        number = vector2.getter().y
                    });
                }
                else
                {
                    propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name));
                    if (i is ExposedNumber number) cache.Add(_ => new() { number = number.getter() });
                    else if (i is ExposedBool boolean) cache.Add(_ => new() { condition = boolean.getter() });
                    else if (i is ExposedString str) cache.Add(_ => new() { str = str.getter() });
                    else if (i is ExposedDropdown dropdown) cache.Add(_ => new() { number = (float)dropdown.getter() });
                    else if (i is ExposedObject obj) cache.Add(_ => new() { obj = obj.getter() });
                    else if (i is ExposedAsset asset) cache.Add(_ => new() { asset = asset.getter() });
                }
            }
            else if (i is ExposedAnchor anchor)
            {
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMin.x"));
                cache.Add(_ => new() { number = anchor.minGetter().x });
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMin.y"));
                cache.Add(_ => new() { number = anchor.minGetter().y });
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMax.x"));
                cache.Add(_ => new() { number = anchor.maxGetter().x });
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMax.y"));
                cache.Add(_ => new() { number = anchor.maxGetter().y });
            }
        }
        propertyDropdown.RefreshShownValue();
    }
    public override float GetNumber(ulong hash) => cache[propertyDropdown.value].Invoke(hash).number;
    public override bool GetCondition(ulong hash) => cache[propertyDropdown.value].Invoke(hash).condition;
    public override string GetString(ulong hash) => cache[propertyDropdown.value].Invoke(hash).str;
    public override MyGameObject GetObject(ulong hash) => cache[propertyDropdown.value].Invoke(hash).obj;
    public override MyAsset GetAsset(ulong hash) => cache[propertyDropdown.value].Invoke(hash).asset;
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
    public override float GetWidth() => rectTransform.rect.width;
}