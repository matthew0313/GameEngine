using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnapPoint_FunctionParam : SnapPoint
{
    [SerializeField] CodeBlock_FunctionParam blockPrefab;
    public string parameterName;
    public override bool IsSnappable(CodeBlock codeBlock) => codeBlock is CodeBlock_FunctionParam block && block.target == ownerBlock && block.parameterName == parameterName;
    public override void Snap(CodeBlock codeBlock)
    {
        codeBlock.Delete();
    }
    protected override void OnSnappedChange()
    {
        base.OnSnappedChange();
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