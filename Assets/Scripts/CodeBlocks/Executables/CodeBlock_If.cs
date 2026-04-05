using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_If : ExecutableCodeBlock
{
    public override string id => "If";

    [SerializeField] RectTransform rectTransform;
    [SerializeField] ConditionSnapPoint condition;
    [SerializeField] ExecutableSnapPoint onTrue, onFinish;
    public override async UniTask Execute(ulong hash)
    {
        if (condition.GetCondition()) await onTrue.Execute(hash);
        await onFinish.Execute(hash);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}