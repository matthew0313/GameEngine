using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HashSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && 
            codeBlock is PropertyCodeBlock propertyBlock && 
            (propertyBlock.propertyType & PropertyType.Hash) > 0;
    }
    protected override void OnSnappedChange()
    {
        unSnapped.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        if(layoutElement != null) layoutElement.minWidth = GetWidth();
    }
    public ulong GetHash(ulong hash)
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetHash(hash);
        }
        return ulong.MaxValue;
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