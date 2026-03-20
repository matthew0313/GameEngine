using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CodeBlockList", menuName = "Scriptables/CodeBlockList")]
public class CodeBlockList : ScriptableObject
{
    public List<CodeBlock> codeBlocks;
}