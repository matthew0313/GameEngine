using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_LerpColor : ColorCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] ColorSnapPoint inputA;
    [SerializeField] ColorSnapPoint inputB;
    [SerializeField] NumericSnapPoint t;
    public override Color GetColor(ulong hash) => Color.Lerp(inputA.GetColor(hash), inputB.GetColor(hash), t.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
        yield return t;
    }
}
