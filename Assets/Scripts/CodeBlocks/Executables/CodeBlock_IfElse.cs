using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_IfElse : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Logic;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] ConditionSnapPoint condition;
    [SerializeField] ExecutableSnapPoint onTrue;
    [SerializeField] ExecutableSnapPoint onFalse;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        if (condition.GetCondition(hash)) await onTrue.Execute(hash);
        else await onFalse.Execute(hash);
        return await onFinish.Execute(hash);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return condition;
        yield return onTrue;
        yield return onFalse;
        yield return onFinish;
    }
}