using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_If : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Logic;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] ConditionSnapPoint condition;
    [SerializeField] ExecutableSnapPoint onTrue;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        if (condition.GetCondition(hash))
        {
            var info = await onTrue.Execute(hash, token);
            if (info.breaked) return info;
        }
        return await onFinish.Execute(hash, token);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return condition;
        yield return onTrue;
        yield return onFinish;
    }
}