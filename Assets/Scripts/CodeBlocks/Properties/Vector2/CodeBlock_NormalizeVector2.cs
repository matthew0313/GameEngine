using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_NormalizeVector2 : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2SnapPoint input;
    public override Vector2 GetVector2(ulong hash) => Vector2.Normalize(input.GetVector2(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return input;
    }
}