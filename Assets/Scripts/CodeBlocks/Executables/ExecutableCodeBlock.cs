using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class ExecutableCodeBlock : CodeBlock
{
    public abstract UniTask Execute();
    public abstract float GetHeight();
}
