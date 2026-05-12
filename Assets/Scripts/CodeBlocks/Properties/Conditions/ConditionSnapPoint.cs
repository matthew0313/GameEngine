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
            propertyBlock.propertyType == PropertyType.Condition;
    }
    protected override void OnSnappedChange()
    {
        toggle.gameObject.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        layoutElement.minWidth = GetWidth();
    }
    public bool GetCondition(ulong hash)
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetCondition(hash);
        }
        else return toggle.isOn;
    }
    public float GetWidth()
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetWidth();
        }
        else return defaultWidth;
    }
}