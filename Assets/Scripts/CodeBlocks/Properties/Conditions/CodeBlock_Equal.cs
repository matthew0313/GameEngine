using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Equal : ConditionCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Condition;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint inputA, inputB;
    public override bool GetCondition(ulong hash) => Mathf.Approximately(inputA.GetNumber(hash), inputB.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
    }
}
