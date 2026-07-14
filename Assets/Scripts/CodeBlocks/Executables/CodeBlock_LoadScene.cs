using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_LoadScene : ExecutableCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] AssetSnapPoint sceneAsset;
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        MyAsset asset = sceneAsset.GetAsset(hash);
        if (!EditorSceneManager.Instance.playMode)
        {
            EditorSceneManager.Instance.AddLog(new(MyLogType.Warning, "Load Scene only works in play mode."));
            return new();
        }
        if (asset is not SceneAsset scene)
        {
            EditorSceneManager.Instance.AddLog(new(MyLogType.Error, $"Asset is not scene asset."));
            return new();
        }
        EditorSceneManager.Instance.OpenSceneAsset(scene);
        return new() { ended = true };
    }
    public override float GetHeight() => rectTransform.rect.height;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return sceneAsset;
    }
}
