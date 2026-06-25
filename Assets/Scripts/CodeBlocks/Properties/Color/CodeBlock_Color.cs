using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Color : ColorCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint inputR;
    [SerializeField] NumericSnapPoint inputG;
    [SerializeField] NumericSnapPoint inputB;
    [SerializeField] NumericSnapPoint inputA;
    public override Color GetColor(ulong hash) => new Color(inputR.GetNumber(hash), inputG.GetNumber(hash), inputB.GetNumber(hash), inputA.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputR;
        yield return inputG;
        yield return inputB;
        yield return inputA;
    }
}
