using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StringSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] TMP_InputField inputField;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && 
            codeBlock is PropertyCodeBlock propertyBlock && 
            ((propertyBlock.propertyType & PropertyType.String) > 0 || (propertyBlock.propertyType & PropertyType.Number) > 0);
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
    public string GetValue(ulong hash)
    {
        if(snapped == null)
        {
            return inputField.text;
        }
        else if (snapped is PropertyCodeBlock propertyBlock)
        {
            if ((propertyBlock.propertyType & PropertyType.String) > 0) return propertyBlock.GetString(hash);
            if ((propertyBlock.propertyType & PropertyType.Number) > 0) return propertyBlock.GetNumber(hash).ToString();
        }
        return string.Empty;
    }
    public float GetWidth()
    {
        if (snapped is StringCodeBlock stringCodeBlock)
        {
            return stringCodeBlock.GetWidth();
        }
        else if (snapped is NumericCodeBlock numericCodeBlock)
        {
            return numericCodeBlock.GetWidth();
        }
        else return defaultWidth;
    }
    public override SnapPointSave Save()
    {
        var tmp = base.Save();
        tmp.data.strings["value"] = inputField.text;
        return tmp;
    }
    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        inputField.text = save.data.strings["value"];
    }
}