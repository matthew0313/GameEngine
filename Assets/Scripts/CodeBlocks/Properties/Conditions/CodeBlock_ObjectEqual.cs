using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_ObjectEqual : ConditionCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Condition;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] ObjectSnapPoint inputA, inputB;
    public override bool GetCondition(ulong hash) => inputA.GetObject(hash) == inputB.GetObject(hash);
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
    }
}
