using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CodeBlock_Round : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint input;
    public override float GetNumber(ulong hash) => Mathf.Round(input.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
    }
}