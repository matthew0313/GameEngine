using System.Collections.Generic;
using UnityEngine;

public abstract class StringCodeBlock : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.String;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Log String",
            ctx =>
            {
                EditorSceneManager.Instance.AddLog(new()
                {
                    type = MyLogType.Info,
                    message = GetString(ulong.MaxValue)
                });
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}
