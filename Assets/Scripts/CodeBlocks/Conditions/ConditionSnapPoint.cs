using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] Toggle toggle;
    public override bool IsSnappable(CodeBlock codeBlock) => base.IsSnappable(codeBlock) && codeBlock is ConditionCodeBlock;
    protected override void OnSnappedChange()
    {
        toggle.gameObject.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        layoutElement.minWidth = GetWidth();
    }
    public bool GetCondition()
    {
        if (snapped is ConditionCodeBlock conditionCodeBlock)
        {
            return conditionCodeBlock.GetCondition();
        }
        else return toggle.isOn;
    }
    public float GetWidth()
    {
        if (snapped is ConditionCodeBlock conditionCodeBlock)
        {
            return conditionCodeBlock.GetWidth();
        }
        else return defaultWidth;
    }
}