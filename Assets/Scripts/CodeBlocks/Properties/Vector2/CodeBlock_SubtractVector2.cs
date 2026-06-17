using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_SubtractVector2 : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2SnapPoint inputA;
    [SerializeField] Vector2SnapPoint inputB;
    public override Vector2 GetVector2(ulong hash) => inputA.GetVector2(hash) - inputB.GetVector2(hash);
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
    }
}