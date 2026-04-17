using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodeBlock_For : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint loopCount;
    [SerializeField] ExecutableSnapPoint onLoop;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    readonly Dictionary<ulong, int> loopIndices = new();
    public int GetLoopIndex(ulong hash)
    {
        if (loopIndices.TryGetValue(hash, out int index)) return index;
        return -1;
    }
    public override async UniTask Execute(ulong hash)
    {
        int count = loopCount.GetIntValue(hash);
        for(int i = 0; i < count; i++)
        {
            loopIndices[hash] = i;
            await onLoop.Execute(hash);
        }
        loopIndices.Remove(hash);
        await onFinish.Execute(hash);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}