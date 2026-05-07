using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_Move : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint moveX, moveY;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        if(owner is MyGameObject go) go.transform.position += new Vector3(moveX.GetValue(hash), moveY.GetValue(hash));
        return await onFinish.Execute(hash);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}