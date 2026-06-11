using UnityEngine;

public class CodeBlock_Round : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Calculation;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint input;
    public override float GetNumber(ulong hash) => Mathf.Round(input.GetNumber(hash));
    public override float GetWidth() => rectTransform.rect.width;
}