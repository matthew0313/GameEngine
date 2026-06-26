using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_FindAsset : AssetCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] StringSnapPoint assetName;
    public override MyAsset GetAsset(ulong hash)
    {
        string tmp = assetName.GetValue(hash);
        return EditorSceneManager.Instance.FindAsset(item => item.name == tmp);
    }
    public override float GetWidth() => rectTransform.rect.width;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return assetName;
    }
}