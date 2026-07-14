using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Instantiate : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] AssetSnapPoint prefabAsset;
    [SerializeField] SnapPoint_InstantiatedObject instantiatedObject;
    [field: SerializeField] public ExecutableSnapPoint onFinish { get; private set; }

    readonly Dictionary<ulong, MyGameObject> instantiatedObjects = new();
    public MyGameObject GetInstantiatedObject(ulong hash) =>
        instantiatedObjects.TryGetValue(hash, out var obj) ? obj : null;

    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        MyAsset asset = prefabAsset.GetAsset(hash);
        if (asset is not PrefabAsset prefab)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Warning,
                message = "Invalid prefab asset"
            });
            return new() { exception = true };
        }
        instantiatedObjects[hash] = prefab.Instantiate();
        var info = await onFinish.Execute(hash, token);
        instantiatedObjects.Remove(hash);
        return info;
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return prefabAsset;
        yield return instantiatedObject;
        yield return onFinish;
    }
}
