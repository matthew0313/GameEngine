using UnityEngine;

public class CodeBlock_TestNumeric : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float value = 10.0f;

    public override float GetNumber(ulong hash) => value;
    public override float GetWidth() => rectTransform.rect.width;
}
