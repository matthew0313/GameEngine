using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_LerpVector2 : Vector2CodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2SnapPoint inputA;
    [SerializeField] Vector2SnapPoint inputB;
    [SerializeField] NumericSnapPoint t;
    public override Vector2 GetVector2(ulong hash) => Vector2.Lerp(inputA.GetVector2(hash), inputB.GetVector2(hash), t.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
        yield return t;
    }
}
