using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodeBlock_ForIndex : NumericCodeBlock
{
    [SerializeField] RectTransform rectTransform;
    public CodeBlock_For target;
    public override bool addable => false;
    public override float GetValue(ulong hash) => target != null ? target.GetLoopIndex(hash) : -1;
    public override float GetWidth() => rectTransform.rect.width;
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.ulongs["target"] = target != null ? target.uid : 0;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        if (save.data.ulongs.TryGetValue("target", out ulong targetUID))
        {
            var targetBlock = owner.codeBlocks.Find(block => block.uid == targetUID);
            if (targetBlock is CodeBlock_For forBlock)
            {
                target = forBlock;
            }
        }
    }
}