using System;
using System.Collections.Generic;
using UnityEngine;

public class CodeTab : EditorTab
{
    ICodeable selected;
    [field:SerializeField] public Transform blockAnchor { get; private set; }
}