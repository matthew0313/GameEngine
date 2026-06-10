using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Codeblock_GetProperty : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Wildcard;
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    public override float GetWidth() => rectTransform.rect.width;
}