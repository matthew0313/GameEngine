using UnityEngine;

public class CodeBlock_FixedDeltaTime : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    public override float GetNumber(ulong hash) => Time.fixedDeltaTime;
    public override float GetWidth() => rectTransform.rect.width;
}
