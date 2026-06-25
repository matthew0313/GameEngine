using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Codeblock_GetHash : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Hash;
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    public override ulong GetHash(ulong hash) => hash;
    public override float GetWidth() => rectTransform.rect.width;
}