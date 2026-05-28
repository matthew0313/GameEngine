using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_OnAwake : CodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Starter;
    [SerializeField] ExecutableSnapPoint onAwake;
    public ExecutableSnapPoint onFinish => onAwake;

    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        MyGameObject obj = owner as MyGameObject;
        if(obj == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = "OnAwake block added in a non-object. Block will not function."
            });
        }
        obj.onAwake += OnAwake;
    }
    ulong testHash = 0, hash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx =>
            {
                onAwake.Execute(testHash++);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    private void OnAwake()
    {
        onAwake.Execute(hash++);
    }
    private void OnDestroy()
    {
        if (owner is MyGameObject obj) obj.onAwake -= OnAwake;
    }
}