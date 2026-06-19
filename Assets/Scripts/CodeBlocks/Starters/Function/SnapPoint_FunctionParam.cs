using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnapPoint_FunctionParam : SnapPoint
{
    [SerializeField] CodeBlock_FunctionParam blockPrefab;
    public string parameterName;
    public override bool IsSnappable(CodeBlock codeBlock) => codeBlock is CodeBlock_FunctionParam block && block.target == ownerBlock;
    public override void Snap(CodeBlock codeBlock)
    {
        var tmp = codeBlock as CodeBlock_FunctionParam;
        var owner = ownerBlock as CodeBlock_Function;
        owner.SwapParameter(parameterName, tmp.parameterName);
        codeBlock.Delete();
    }
    protected override void OnSnappedChange()
    {
        base.OnSnappedChange();
        if (!enabled) return;
        if(snapped == null)
        {
            var tmp = Instantiate(blockPrefab);
            tmp.BindTarget(ownerBlock as CodeBlock_Function, parameterName);
            tmp.Set(ownerBlock.owner);
            ownerBlock.owner.codeBlocks.Add(tmp);
            base.Snap(tmp);
        }
    }
}