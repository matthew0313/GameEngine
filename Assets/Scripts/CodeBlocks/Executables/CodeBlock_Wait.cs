using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_Wait : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint duration;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        await UniTask.WaitForSeconds(duration.GetValue(hash));
        return await onFinish.Execute(hash);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}