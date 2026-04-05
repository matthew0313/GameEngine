using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class ExecutableCodeBlock : CodeBlock
{
    public abstract UniTask Execute(ulong hash);
    public abstract float GetHeight();
}
