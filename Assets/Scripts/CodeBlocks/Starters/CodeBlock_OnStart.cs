using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_OnStart : CodeBlock
{
    [SerializeField] ExecutableSnapPoint onStart;
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        
    }
}