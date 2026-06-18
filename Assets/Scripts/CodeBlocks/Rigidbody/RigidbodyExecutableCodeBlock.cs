using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Base for executable Rigidbody blocks (Movement category). Addable only to a
/// Rigidbody object or a PrefabAsset whose origin is one.</summary>
public abstract class RigidbodyExecutableCodeBlock : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Movement;
    [SerializeField] protected RectTransform rectTransform;
    [field: SerializeField] public ExecutableSnapPoint onFinish { get; private set; }

    public override bool IsAddable(ICodeable codeable) =>
        base.IsAddable(codeable) &&
        (codeable is MyGameObject_Rigidbody ||
        (codeable is PrefabAsset prefab && prefab.prefabOrigin is MyGameObject_Rigidbody));

    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        if (owner is MyGameObject_Rigidbody rb) Apply(rb, hash);
        else
        {
            EditorSceneManager.Instance.AddLog(new MyLog(MyLogType.Error, "Rigidbody block executed in non-rigidbody object."));
            return new() { exception = true };
        }
        return await onFinish.Execute(hash);
    }
    protected abstract void Apply(MyGameObject_Rigidbody rb, ulong hash);

    public override float GetHeight() => rectTransform.rect.height + onFinish.GetHeight();
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        foreach (var snap in Inputs()) yield return snap;
        yield return onFinish;
    }
    protected virtual IEnumerable<SnapPoint> Inputs() { yield break; }
}
