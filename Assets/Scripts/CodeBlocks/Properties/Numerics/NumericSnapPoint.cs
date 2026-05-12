using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumericSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] TMP_InputField inputField;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && 
            codeBlock is PropertyCodeBlock propertyBlock && 
            (propertyBlock.propertyType & PropertyType.Number) > 0;
    }
    protected override void OnSnappedChange()
    {
        inputField.gameObject.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        layoutElement.minWidth = GetWidth();
    }
    public float GetNumber(ulong hash)
    {
        if (snapped == null)
        {
            if (float.TryParse(inputField.text, out float value)) return value;
        }
        else if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetNumber(hash);
        }
        return 0.0f;
    }
    public int GetIntNumber(ulong hash) => Mathf.FloorToInt(GetNumber(hash));
    public float GetWidth()
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetWidth();
        }
        else return defaultWidth;
    }
}