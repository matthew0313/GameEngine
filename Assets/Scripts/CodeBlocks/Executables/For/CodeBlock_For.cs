using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodeBlock_For : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Logic;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint loopCount;
    [SerializeField] ExecutableSnapPoint onLoop;
    [SerializeField] SnapPoint_ForIndex index;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    readonly Dictionary<ulong, int> loopIndices = new();
    public int GetLoopIndex(ulong hash)
    {
        if (loopIndices.TryGetValue(hash, out int index)) return index;
        return -1;
    }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        int count = loopCount.GetIntNumber(hash);
        for(int i = 0; i < count; i++)
        {
            loopIndices[hash] = i;
            var info = await onLoop.Execute(hash, token);
            if (info.breaked) break;
        }
        loopIndices.Remove(hash);
        return await onFinish.Execute(hash, token);
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return index;
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}