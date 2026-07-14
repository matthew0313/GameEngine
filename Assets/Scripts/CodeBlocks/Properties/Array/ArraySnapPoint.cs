using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArraySnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    [SerializeField] TMP_Text setAssetText;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) &&
            codeBlock is PropertyCodeBlock propertyBlock &&
            (propertyBlock.propertyType & PropertyType.Array) > 0;
    }
    private void Update()
    {
        if (layoutElement != null) layoutElement.minWidth = GetWidth();
    }
    protected override void OnSnappedChange()
    {
        unSnapped.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    public List<Wildcard> GetArray(ulong hash)
    {
        if (snapped == null)
        {
            return new();
        }
        else if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetArray(hash);
        }
        return null;
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
