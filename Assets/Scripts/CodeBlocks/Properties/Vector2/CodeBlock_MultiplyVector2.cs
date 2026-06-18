using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_MultiplyVector2 : Vector2CodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2SnapPoint inputA;
    [SerializeField] NumericSnapPoint inputB;
    public override Vector2 GetVector2(ulong hash) => inputA.GetVector2(hash) * inputB.GetNumber(hash);
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
    }
}