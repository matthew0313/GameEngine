using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_OnFixedUpdate : CodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Starter;
    [SerializeField] ExecutableSnapPoint onFixedUpdate;
    public ExecutableSnapPoint onFinish => onFixedUpdate;
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        MyGameObject obj = owner as MyGameObject;
        if(obj != null) obj.onFixedUpdate += OnFixedUpdate;
    }
    ulong testHash = 0, hash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx =>
            {
                onFixedUpdate.Execute(testHash++, CancellationToken.None);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    private void OnFixedUpdate()
    {
        onFixedUpdate.Execute(hash++, EditorSceneManager.Instance.playToken);
    }
    public override void Delete()
    {
        if (owner is MyGameObject obj) obj.onFixedUpdate -= OnFixedUpdate;
        base.Delete();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onFixedUpdate;
    }
}