using System.Collections.Generic;
using UnityEngine;

public abstract class StringCodeBlock : CodeBlock
{
    public abstract string GetValue(ulong hash);
    public abstract float GetWidth();
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Log Value",
            ctx =>
            {
                EditorSceneManager.Instance.AddLog(new()
                {
                    type = MyLogType.Info,
                    message = GetValue(ulong.MaxValue)
                });
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}
