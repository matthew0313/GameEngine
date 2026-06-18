using Cysharp.Threading.Tasks;
using System.Threading;
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
        if(obj != null) obj.onAwake += OnAwake;
    }
    ulong testHash = 0, hash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx =>
            {
                onAwake.Execute(testHash++, CancellationToken.None);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    private void OnAwake()
    {
        onAwake.Execute(hash++, EditorSceneManager.Instance.playToken);
    }
    public override void Delete()
    {
        if (owner is MyGameObject obj) obj.onAwake -= OnAwake;
        base.Delete();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onAwake;
    }
}