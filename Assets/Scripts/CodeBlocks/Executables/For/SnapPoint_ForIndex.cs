using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnapPoint_ForIndex : SnapPoint
{
    [SerializeField] CodeBlock_ForIndex blockPrefab;
    public override bool IsSnappable(CodeBlock codeBlock) => false;
    protected override void OnSnappedChange()
    {
        base.OnSnappedChange();
        if(snapped == null)
        {
            var tmp = Instantiate(blockPrefab);
            tmp.owner = ownerBlock.owner;
            ownerBlock.owner.codeBlocks.Add(tmp);
        }
    }
}