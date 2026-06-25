using System.Collections.Generic;
using UnityEngine;

public abstract class ColorCodeBlock : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Color;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Log Color",
            ctx =>
            {
                Color tmp = GetColor(0);
                EditorSceneManager.Instance.AddLog(new()
                {
                    type = MyLogType.Info,
                    message = $"RGBA({tmp.r}, {tmp.g}, {tmp.b}, {tmp.a})"
                });
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}
