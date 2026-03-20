using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICodeable
{
    public List<CodeBlock> codeBlocks { get; }
    public IEnumerable<CodeBlock> GetAvailableBlocks();
}