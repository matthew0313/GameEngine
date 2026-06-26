using Cysharp.Threading.Tasks;
using System.Threading;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Codeblock_SetVariable : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] ObjectSnapPoint targetObject;
    [SerializeField] StringSnapPoint variableName;
    [SerializeField] WildcardSnapPoint value;
    [field: SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        targetObject.SetObject(owner as MyGameObject);
    }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if(target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for SetVariable"
            });
            return new()
            {
                exception = true
            };
        }
        string name = variableName.GetValue(hash);
        if(target.variables.ContainsKey(name)) target.variables[name] = value.GetWildcard(hash);
        return await onFinish.Execute(hash, token);
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return targetObject;
        yield return variableName;
        yield return value;
        yield return onFinish;
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}