using UnityEngine;

public class CodeBlock_DeltaTime : NumericCodeBlock
{
    [SerializeField] RectTransform rectTransform;
    public override float GetNumber(ulong hash) => Time.deltaTime;
    public override float GetWidth() => rectTransform.rect.width;
}
