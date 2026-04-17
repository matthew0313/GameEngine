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
    public override void Snap(CodeBlock codeBlock)
    {
        if (codeBlock == null || !IsSnappable(codeBlock)) return;
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
        EditorSceneManager.Instance.scriptGrid.BindToGrid(snapped);
        snapped.transform.SetParent(snapAnchor);
        snapped.transform.localPosition = Vector3.zero;
        OnSnappedChange();
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