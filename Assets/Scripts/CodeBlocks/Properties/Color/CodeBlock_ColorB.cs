using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_ColorB : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] ColorSnapPoint input;
    public override float GetNumber(ulong hash) => input.GetColor(hash).b;
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
    }
}
