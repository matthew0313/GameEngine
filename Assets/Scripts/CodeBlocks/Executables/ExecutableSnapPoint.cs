using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExecutableSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultHeight = 0.0f;
    public override bool IsSnappable(CodeBlock codeBlock) => base.IsSnappable(codeBlock) && codeBlock is ExecutableCodeBlock;
    private void Update()
    {
        if(layoutElement != null) layoutElement.minHeight = GetHeight();
    }
    public async UniTask Execute(ulong hash)
    {
        if (snapped is ExecutableCodeBlock executableCodeBlock)
        {
            await executableCodeBlock.Execute(hash);
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