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
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name));
                if (i is ExposedNumber number) cache.Add(_ => new() { number = number.getter() });
                else if (i is ExposedBool boolean) cache.Add(_ => new() { condition = boolean.getter() });
                else if (i is ExposedString str) cache.Add(_ => new() { str = str.getter() });
                else if (i is ExposedDropdown dropdown) cache.Add(_ => new() { number = (float)dropdown.getter() });
                else if (i is ExposedObject obj) cache.Add(_ => new() { obj = obj.getter() });
                else if (i is ExposedAsset asset) cache.Add(_ => new() { asset = asset.getter() });
                else if (i is ExposedVector2 vector2) cache.Add(_ => new() { vector2 = vector2.getter() });
                else if (i is ExposedColor color) cache.Add(_ => new() { color = color.getter() });
            }
            else if (i is ExposedAnchor anchor)
            {
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMin"));
                cache.Add(_ => new() { vector2 = anchor.minGetter() });
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMax"));
                cache.Add(_ => new() { vector2 = anchor.maxGetter() });
            }
        }
        propertyDropdown.RefreshShownValue();
    }
    public override float GetNumber(ulong hash) => cache[propertyDropdown.value].Invoke(hash).number;
    public override bool GetCondition(ulong hash) => cache[propertyDropdown.value].Invoke(hash).condition;
    public override string GetString(ulong hash) => cache[propertyDropdown.value].Invoke(hash).str;
    public override MyGameObject GetObject(ulong hash) => cache[propertyDropdown.value].Invoke(hash).obj;
    public override MyAsset GetAsset(ulong hash) => cache[propertyDropdown.value].Invoke(hash).asset;
    public override Vector2 GetVector2(ulong hash) => cache[propertyDropdown.value].Invoke(hash).vector2;
    public override Color GetColor(ulong hash) => cache[propertyDropdown.value].Invoke(hash).color;
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.integers["property"] = propertyDropdown.value;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        if (save.data.integers.TryGetValue("property", out int property)) propertyDropdown.value = property;
    }
    public override float GetWidth() => rectTransform.rect.width;
}