using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Clamp : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint input;
    [SerializeField] NumericSnapPoint min;
    [SerializeField] NumericSnapPoint max;
    public override float GetNumber(ulong hash) =>
        Mathf.Clamp(input.GetNumber(hash), min.GetNumber(hash), max.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
        yield return min;
        yield return max;
    }
}
