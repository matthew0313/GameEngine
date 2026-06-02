using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] Toggle toggle;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && 
            codeBlock is PropertyCodeBlock propertyBlock && 
            (propertyBlock.propertyType & PropertyType.Condition) > 0;
    }
    protected override void OnSnappedChange()
    {
        toggle.gameObject.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        if(layoutElement != null) layoutElement.minWidth = GetWidth();
    }
    public bool GetCondition(ulong hash)
    {
        if (snapped == null)
        {
            return toggle.isOn;
        }
        else if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetCondition(hash);
        }
        else return false;
    }
    public float GetWidth()
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetWidth();
        }
        else return defaultWidth;
    }
    public override SnapPointSave Save()
    {
        var save = base.Save();
        save.data.bools["value"] = toggle.isOn;
        return save;
    }
    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        toggle.isOn = save.data.bools["value"];
    }
}