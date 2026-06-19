using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_PositionOf : Vector2CodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Movement;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] ObjectSnapPoint targetObject;
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        targetObject.SetObject(owner as MyGameObject);
    }
    public override Vector2 GetVector2(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target != null) return target.transform.localPosition;
        return base.GetVector2(hash);
    }
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return targetObject;
    }
}
