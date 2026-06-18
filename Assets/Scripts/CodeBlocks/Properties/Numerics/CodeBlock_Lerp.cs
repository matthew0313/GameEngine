using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Lerp : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint inputA;
    [SerializeField] NumericSnapPoint inputB;
    [SerializeField] NumericSnapPoint t;
    public override float GetNumber(ulong hash) => Mathf.Lerp(inputA.GetNumber(hash), inputB.GetNumber(hash), t.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
        yield return t;
    }
}
