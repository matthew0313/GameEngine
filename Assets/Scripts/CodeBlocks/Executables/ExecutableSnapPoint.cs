using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExecutableSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultHeight = 0.0f;
    public override bool IsSnappable(CodeBlock codeBlock) => codeBlock is ExecutableCodeBlock;
    private void Update()
    {
        layoutElement.minHeight = GetHeight();
    }
    public void Execute()
    {
        if (snapped is ExecutableCodeBlock executableCodeBlock)
        {
            executableCodeBlock.Execute();
        }
    }
    public float GetHeight()
    {
        if(snapped is ExecutableCodeBlock executableCodeBlock)
        {
            return executableCodeBlock.GetHeight();
        }
        else return defaultHeight;
    }
}