using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICodeable
{
    public Vector2 lastOffset { get; set; }
    public float lastZoom { get; set; }
    public List<CodeBlock> codeBlocks { get; }
    public IEnumerable<CodeBlock> GetAvailableBlocks();
}