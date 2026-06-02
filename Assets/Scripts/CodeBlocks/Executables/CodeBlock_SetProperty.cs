using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
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
                propertyDropdown.options.Add(new TMP_Dropdown.OptionData(property.name));
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
                if(index-- == 0)
                {
                    if (property is ExposedNumber number) number.setter(valueSnapPoint.GetNumber(hash));
                    else if (property is ExposedBool boolean) boolean.setter(valueSnapPoint.GetCondition(hash));
                    else if (property is ExposedString str) str.setter(valueSnapPoint.GetString(hash));
                    else if (property is ExposedDropdown dropdown) dropdown.setter((int)valueSnapPoint.GetNumber(hash));
                    else if (property is ExposedObject obj) obj.setter(valueSnapPoint.GetObject(hash));
                    else if (property is ExposedAsset asset) asset.setter(valueSnapPoint.GetAsset(hash));
                    break;
                }
            }
        }
        inspectable.onInspectorChange?.Invoke();
        return await onFinish.Execute(hash);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}