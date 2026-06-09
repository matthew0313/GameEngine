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
    [SerializeField] WildcardSnapPoint valueSnapPoint;
    [field: SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override bool IsAddable(ICodeable codeable)
    {
        if (base.IsAddable(codeable) && codeable is IInspectable inspectable)
        {
            foreach(var i in inspectable.GetElements())
            {
                if(i is ExposedProperty)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        IInspectable inspectable = owner as IInspectable;
        propertyDropdown.ClearOptions();
        foreach(var i in inspectable.GetElements())
        {
            if(i is ExposedProperty property)
            {
                if(i is ExposedVector2 vector2)
                {
                    propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name + ".x"));
                    propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name + ".y"));
                }
                else propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name));
            }
            else if(i is ExposedAnchor anchor)
            {
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMin.x"));
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMin.y"));
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMax.x"));
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData("anchorMax.y"));
            }
        }
        propertyDropdown.RefreshShownValue();
    }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        int index = propertyDropdown.value;
        IInspectable inspectable = owner as IInspectable;
        foreach (var i in inspectable.GetElements())
        {
            if (i is ExposedProperty property)
            {
                if(property is ExposedVector2 vector2)
                {
                    if (index == 0) vector2.setter(new Vector2(valueSnapPoint.GetNumber(hash), vector2.getter().y));
                    else if (index == 1) vector2.setter(new Vector2(vector2.getter().x, valueSnapPoint.GetNumber(hash)));
                    index -= 2;
                }
                else if (property is ExposedNumber number)
                {
                    if (index == 0) number.setter(valueSnapPoint.GetNumber(hash));
                    index--;
                }
                else if (property is ExposedBool boolean)
                {
                    if (index == 0) boolean.setter(valueSnapPoint.GetCondition(hash));
                    index--;
                }
                else if (property is ExposedString str)
                {
                    if (index == 0) str.setter(valueSnapPoint.GetString(hash));
                    index--;
                }
                else if (property is ExposedDropdown dropdown)
                {
                    if (index == 0) dropdown.setter((int)valueSnapPoint.GetNumber(hash));
                    index--;
                }
                else if (property is ExposedObject obj)
                {
                    if (index == 0) obj.setter(valueSnapPoint.GetObject(hash));
                    index--;
                }
                else if (property is ExposedAsset asset)
                {
                    if (index == 0) asset.setter(valueSnapPoint.GetAsset(hash));
                    index--;
                }
            }
            else if(i is ExposedAnchor anchor)
            {
                float value = valueSnapPoint.GetNumber(hash);
                Vector2 anchorMin = anchor.minGetter(), anchorMax = anchor.maxGetter();

                if (index == 0) anchor.minSetter(new Vector2(value, anchorMin.y));
                else if (index == 1) anchor.minSetter(new Vector2(anchorMin.x, value));
                else if (index == 2) anchor.maxSetter(new Vector2(value, anchorMax.y));
                else if (index == 3) anchor.maxSetter(new Vector2(anchorMax.x, value));
                index -= 4;
            }
            if (index < 0) break;
        }
        inspectable.onInspectorChange?.Invoke();
        return await onFinish.Execute(hash);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}