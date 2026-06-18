using Cysharp.Threading.Tasks;
using System;
using System.Threading;
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
    public override void Snap(CodeBlock codeBlock)
    {
        if (codeBlock == null) return;
        if (codeBlock.snappedPoint != null) codeBlock.snappedPoint.Detach();
        if (snapped != null)
        {
            if(codeBlock is IOnFinish onFinish)
            {
                onFinish.onFinish.Snap(snapped);
            }
            else
            {
                snapped.transform.position += Vector3.right * 30.0f;
                Detach();
            }
        }
        snapped = codeBlock;
        snapped.snappedPoint = this;
        snapped.transform.SetParent(snapAnchor, true);
        snapped.transform.localScale = Vector3.one;
        snapped.transform.localPosition = Vector3.zero;
        OnSnappedChange();
    }
    public async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        if (token.IsCancellationRequested) return new() { breaked = true };
        if (snapped is ExecutableCodeBlock executableCodeBlock)
        {
            return await executableCodeBlock.Execute(hash, token);
        }
        return new();
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