using System.Collections.Generic;
using UnityEngine;

public abstract class AssetCodeBlock : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Asset;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Log Asset Name",
            ctx =>
            {
                var asset = GetAsset(0);
                EditorSceneManager.Instance.AddLog(new()
                {
                    type = MyLogType.Info,
                    message = asset != null ? asset.name : "null"
                });
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
}