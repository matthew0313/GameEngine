using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_OnUpdate : CodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Starter;
    [SerializeField] ExecutableSnapPoint onUpdate;
    public ExecutableSnapPoint onFinish => onUpdate;

    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        MyGameObject obj = owner as MyGameObject;
        if(obj == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = "OnUpdate block added in a non-object. Block will not function."
            });
        }
        obj.onUpdate += OnUpdate;
    }
    ulong testHash = 0, hash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx =>
            {
                onUpdate.Execute(testHash++);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    private void OnUpdate()
    {
        onUpdate.Execute(hash++);
    }
    private void OnDestroy()
    {
        if (owner is MyGameObject obj) obj.onUpdate -= OnUpdate;
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onUpdate;
    }
}