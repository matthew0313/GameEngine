using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Vector2X : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2SnapPoint input;
    public override float GetNumber(ulong hash) => input.GetVector2(hash).x;
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
    }
}
