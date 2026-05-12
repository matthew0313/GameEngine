using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionCodeBlock : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Condition;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Log Condition",
            ctx =>
            {
                EditorSceneManager.Instance.AddLog(new()
                {
                    type = MyLogType.Info,
                    message = GetCondition(ulong.MaxValue).ToString()
                });
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}