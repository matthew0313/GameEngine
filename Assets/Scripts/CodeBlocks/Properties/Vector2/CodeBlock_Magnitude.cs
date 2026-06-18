using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Magnitude : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2SnapPoint input;
    public override float GetNumber(ulong hash) => input.GetVector2(hash).magnitude;
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
    }
}
