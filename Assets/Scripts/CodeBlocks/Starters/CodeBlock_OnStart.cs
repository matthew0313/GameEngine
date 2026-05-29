using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_OnStart : CodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Starter;
    [SerializeField] ExecutableSnapPoint onStart;
    public ExecutableSnapPoint onFinish => onStart;

    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        MyGameObject obj = owner as MyGameObject;
        if(obj == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = "OnStart block added in a non-object. Block will not function."
            });
        }
        obj.onStart += OnStart;
    }
    ulong testHash = 0, hash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx =>
            {
                onStart.Execute(testHash++);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    private void OnStart()
    {
        onStart.Execute(hash++);
    }
    private void OnDestroy()
    {
        if (owner is MyGameObject obj) obj.onStart -= OnStart;
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onStart;
    }
}