using System.Collections.Generic;
using UnityEngine;

public abstract class ArrayCodeBlock : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Array;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Log Array Size",
            ctx =>
            {
                var tmp = GetArray(0);
                if(tmp == null) EditorSceneManager.Instance.AddLog(new MyLog(MyLogType.Warning, "Array is null"));
                else EditorSceneManager.Instance.AddLog(new MyLog(MyLogType.Info, $"Array Size: {tmp.Count}"));
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}