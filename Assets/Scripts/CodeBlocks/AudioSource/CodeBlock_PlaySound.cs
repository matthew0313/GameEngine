using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_PlaySound : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;
    [SerializeField] RectTransform rectTransform;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override bool IsAddable(ICodeable codeable) => codeable is MyGameObject_AudioSource || codeable is PrefabAsset prefab && prefab.prefabOrigin is MyGameObject_AudioSource;
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        MyGameObject_AudioSource audioSource = owner is PrefabAsset prefab ? prefab.prefabOrigin as MyGameObject_AudioSource : owner as MyGameObject_AudioSource;
        if(audioSource == null)
        {
            EditorSceneManager.Instance.AddLog(new MyLog(MyLogType.Error, "PlaySound block executed in non-AudioSource."));
            return new() { exception = true };
        }
        audioSource.Play();
        return await onFinish.Execute(hash, token);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onFinish;
    }
}