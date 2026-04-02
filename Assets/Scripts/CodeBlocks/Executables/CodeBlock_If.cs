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
    public override async UniTask Execute()
    {
        if (condition.GetCondition()) await onTrue.Execute();
        await onFinish.Execute();
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}