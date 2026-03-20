using UnityEngine;

public class CodeBlock_TestNumeric : NumericCodeBlock
{
    [SerializeField] RectTransform rectTransform;
    public override string id => "TestNumeric";

    public override float GetValue() => 10;

    public override float GetWidth() => rectTransform.rect.width;
}
