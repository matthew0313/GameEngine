using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Divide : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint inputA;
    [SerializeField] NumericSnapPoint inputB;
    public override float GetNumber(ulong hash) => inputA.GetNumber(hash) / inputB.GetNumber(hash);
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
    }
}