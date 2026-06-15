using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnapPoint_ForIndex : SnapPoint
{
    [SerializeField] CodeBlock_ForIndex blockPrefab;
    public override bool IsSnappable(CodeBlock codeBlock) => codeBlock is CodeBlock_ForIndex block && block.target == ownerBlock;
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
            tmp.BindTarget(ownerBlock as CodeBlock_For);
            tmp.Set(ownerBlock.owner);
            ownerBlock.owner.codeBlocks.Add(tmp);
            base.Snap(tmp);
        }
    }
}