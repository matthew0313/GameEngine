using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_ObjectName : StringCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] ObjectSnapPoint input;
    public override string GetString(ulong hash)
    {
        MyGameObject obj = input.GetObject(hash);
        return obj != null ? obj.name : "";
    }
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
    }
}
