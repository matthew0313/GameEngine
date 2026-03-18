using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CodeBlockList", menuName = "Scriptables/CodeBlockList", order = 1)]
public class CodeBlockList : ScriptableObject
{
    [SerializeField] List<CodeBlock> codeBlocks;
    public CodeBlock IDToBlock(string id)
    {
        foreach (var block in codeBlocks)
        {
            if (block.id == id)
                return block;
        }
        return null;
    }
}