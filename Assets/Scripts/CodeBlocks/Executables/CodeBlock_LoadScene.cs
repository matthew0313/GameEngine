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
    [SerializeField] StringSnapPoint sceneName;
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        string tmp = sceneName.GetValue(hash);
        if (!EditorSceneManager.Instance.playMode)
        {
            EditorSceneManager.Instance.AddLog(new(MyLogType.Warning, "Load Scene only works in play mode."));
            return new();
        }
        if (EditorSceneManager.Instance.FindAsset(item => item is SceneAsset && item.name == tmp) is not SceneAsset scene)
        {
            EditorSceneManager.Instance.AddLog(new(MyLogType.Error, $"Load Scene: no scene asset named '{tmp}'."));
            return new();
        }
        EditorSceneManager.Instance.OpenSceneAsset(scene);
        return new() { ended = true };
    }
    public override float GetHeight() => rectTransform.rect.height;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return sceneName;
    }
}
