using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumericSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] TMP_InputField inputField;
    public override bool IsSnappable(CodeBlock codeBlock) => base.IsSnappable(codeBlock) && codeBlock is NumericCodeBlock;
    protected override void OnSnappedChange()
    {
        inputField.gameObject.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        layoutElement.minWidth = GetWidth();
    }
    public float GetValue()
    {
        if (snapped is NumericCodeBlock numericCodeBlock)
        {
            return numericCodeBlock.GetValue();
        }
        else if (float.TryParse(inputField.text, out float value))
        {
            return value;
        }
        return 0.0f;
    }
    public float GetWidth()
    {
        if (snapped is NumericCodeBlock numericCodeBlock)
        {
            return numericCodeBlock.GetWidth();
        }
        else return defaultWidth;
    }
}