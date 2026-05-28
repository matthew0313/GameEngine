using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeBlock_TestCondition : ConditionCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] bool condition = true;
    public override bool GetCondition(ulong hash) => condition;
    public override float GetWidth() => rectTransform.rect.width;
}