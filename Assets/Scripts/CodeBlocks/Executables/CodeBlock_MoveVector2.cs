using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodeBlock_MoveVector2 : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Movement;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2SnapPoint move;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override bool IsAddable(ICodeable codeable) => codeable is MyGameObject || codeable is PrefabAsset;
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        MyGameObject owner = this.owner is PrefabAsset prefab ? prefab.prefabOrigin : this.owner as MyGameObject;
        if (owner == null)
        {
            EditorSceneManager.Instance.AddLog(new MyLog(MyLogType.Error, "Movement block executed in non-object."));
            return new() { exception = true };
        }
        owner.transform.position += (Vector3)move.GetVector2(hash);
        return await onFinish.Execute(hash, token);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return move;
        yield return onFinish;
    }
}
