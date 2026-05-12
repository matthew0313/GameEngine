using System.Collections.Generic;
using UnityEngine;

public abstract class NumericCodeBlock : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Number;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Log Number",
            ctx =>
            {
                EditorSceneManager.Instance.AddLog(new()
                {
                    type = MyLogType.Info,
                    message = GetNumber(ulong.MaxValue).ToString()
                });
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}
