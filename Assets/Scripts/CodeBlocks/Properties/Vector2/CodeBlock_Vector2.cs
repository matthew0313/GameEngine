using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Vector2 : Vector2CodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint inputX;
    [SerializeField] NumericSnapPoint inputY;
    public override Vector2 GetVector2(ulong hash) => new Vector2(inputX.GetNumber(hash), inputY.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return inputX;
        yield return inputY;
    }
}