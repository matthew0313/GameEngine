using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Not : ConditionCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Condition;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] ConditionSnapPoint input;
    public override bool GetCondition(ulong hash) => !input.GetCondition(hash);
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
    }
}
