using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_HashEqual : ConditionCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Condition;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] HashSnapPoint inputA, inputB;
    public override bool GetCondition(ulong hash) => inputA.GetHash(hash) == inputB.GetHash(hash);
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputA;
        yield return inputB;
    }
}
