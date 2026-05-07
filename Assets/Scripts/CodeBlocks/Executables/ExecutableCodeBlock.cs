using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public abstract class ExecutableCodeBlock : CodeBlock
{
    public abstract UniTask<ExecutionFinishedInfo> Execute(ulong hash);
    public abstract float GetHeight();
    ulong testHash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx =>
            {
                Execute(testHash++);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}
public struct ExecutionFinishedInfo
{
    public bool breaked;
}