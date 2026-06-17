using System.Collections.Generic;
using UnityEngine;

public abstract class Vector2CodeBlock : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Vector2;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Log Vector",
            ctx =>
            {
                Vector2 tmp = GetVector2(0);
                EditorSceneManager.Instance.AddLog(new()
                {
                    type = MyLogType.Info,
                    message = $"({tmp.x}, {tmp.y})"
                });
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}
