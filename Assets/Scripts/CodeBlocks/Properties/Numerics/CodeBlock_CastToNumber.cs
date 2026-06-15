using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_CastToNumber : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] WildcardSnapPoint input;
    public override float GetNumber(ulong hash) => input.GetNumber(hash);
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
    }
}