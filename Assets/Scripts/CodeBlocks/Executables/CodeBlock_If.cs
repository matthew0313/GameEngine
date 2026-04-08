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