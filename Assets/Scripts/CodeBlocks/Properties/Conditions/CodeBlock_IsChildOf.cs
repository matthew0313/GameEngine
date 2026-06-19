using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_IsChildOf : ConditionCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Condition;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] ObjectSnapPoint child, parent;
    public override bool GetCondition(ulong hash)
    {
        MyGameObject childObject = child.GetObject(hash);
        MyGameObject parentObject = parent.GetObject(hash);
        return childObject != null && parentObject != null && childObject.IsChildOf(parentObject);
    }
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return child;
        yield return parent;
    }
}
