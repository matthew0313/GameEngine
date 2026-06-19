using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnapPoint_CollidedObject : SnapPoint
{
    [SerializeField] CodeBlock_CollidedObject blockPrefab;
    public override bool IsSnappable(CodeBlock codeBlock) => codeBlock is CodeBlock_CollidedObject block && block.target == ownerBlock;
    public override void Snap(CodeBlock codeBlock)
    {
        codeBlock.Delete();
    }
    protected override void OnSnappedChange()
    {
        base.OnSnappedChange();
        if (!enabled) return;
        if (snapped == null)
        {
            var tmp = Instantiate(blockPrefab);
            tmp.BindTarget(ownerBlock as CodeBlock_OnTrigger);
            tmp.Set(ownerBlock.owner);
            ownerBlock.owner.codeBlocks.Add(tmp);
            base.Snap(tmp);
        }
    }
}
