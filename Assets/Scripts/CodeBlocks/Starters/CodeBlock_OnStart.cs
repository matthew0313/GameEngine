using Cysharp.Threading.Tasks;
using System.Threading;
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
        if(obj != null) obj.onStart += OnStart;
    }
    ulong testHash = 0, hash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx =>
            {
                onStart.Execute(testHash++, CancellationToken.None);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    private void OnStart()
    {
        onStart.Execute(hash++, EditorSceneManager.Instance.playToken);
    }
    public override void Delete()
    {
        if (owner is MyGameObject obj) obj.onStart -= OnStart;
        base.Delete();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onStart;
    }
}