using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_If : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] ConditionSnapPoint condition;
    [SerializeField] ExecutableSnapPoint onTrue;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        if (condition.GetCondition(hash)) await onTrue.Execute(hash);
        return await onFinish.Execute(hash);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}