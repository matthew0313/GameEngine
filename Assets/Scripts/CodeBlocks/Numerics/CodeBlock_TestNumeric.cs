using UnityEngine;

public class CodeBlock_TestNumeric : NumericCodeBlock
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float value = 10.0f;
    public override string id => "TestNumeric";

    public override float GetValue() => value;
    public override float GetWidth() => rectTransform.rect.width;
}
