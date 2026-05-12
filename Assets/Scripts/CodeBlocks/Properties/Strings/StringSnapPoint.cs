using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StringSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] TMP_InputField inputField;
    public override bool IsSnappable(CodeBlock codeBlock) => base.IsSnappable(codeBlock) && codeBlock is PropertyCodeBlock propertyBlock && (propertyBlock.propertyType == PropertyType.String || propertyBlock.propertyType == PropertyType.Number);
    protected override void OnSnappedChange()
    {
        inputField.gameObject.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        layoutElement.minWidth = GetWidth();
    }
    public string GetValue(ulong hash)
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            if(propertyBlock.propertyType == PropertyType.Number) return propertyBlock.GetNumber(hash).ToString();
            return propertyBlock.GetString(hash);
        }
        else if (!string.IsNullOrEmpty(inputField.text))
        {
            return inputField.text;
        }
        return string.Empty;
    }
    public float GetWidth()
    {
        if (snapped is StringCodeBlock stringCodeBlock)
        {
            return stringCodeBlock.GetWidth();
        }
        else if(snapped is NumericCodeBlock numericCodeBlock)
        {
            return numericCodeBlock.GetWidth();
        }
        else return defaultWidth;
    }
}