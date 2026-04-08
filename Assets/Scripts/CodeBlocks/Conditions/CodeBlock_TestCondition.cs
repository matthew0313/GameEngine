using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeBlock_TestCondition : ConditionCodeBlock
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] bool condition = true;
    public override bool GetCondition() => condition;
    public override float GetWidth() => rectTransform.rect.width;
}