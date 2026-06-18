using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public abstract class CodeBlock_OnTrigger : CodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Starter;

    [SerializeField] ExecutableSnapPoint onTrigger;
    [SerializeField] SnapPoint_CollidedObject collidedObject;
    public ExecutableSnapPoint onFinish => onTrigger;

    public override bool IsAddable(ICodeable codeable) =>
        base.IsAddable(codeable) &&
        (codeable is MyGameObject_Rigidbody ||
        (codeable is PrefabAsset prefab && prefab.prefabOrigin is MyGameObject_Rigidbody));

    readonly Dictionary<ulong, MyGameObject> collidedObjects = new();
    public MyGameObject GetCollidedObject(ulong hash) =>
        collidedObjects.TryGetValue(hash, out var obj) ? obj : null;

    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        if (owner is MyGameObject_Rigidbody rb) Subscribe(rb);
    }
    public override void Delete()
    {
        if (owner is MyGameObject_Rigidbody rb) Unsubscribe(rb);
        base.Delete();
    }
    protected abstract void Subscribe(MyGameObject_Rigidbody rb);
    protected abstract void Unsubscribe(MyGameObject_Rigidbody rb);

    ulong testHash = 0, hash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx => { onTrigger.Execute(testHash++); });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    protected void OnObjectTrigger(Collider2D collider)
    {
        Run(collider != null ? collider.GetComponentInParent<MyGameObject>() : null).Forget();
    }
    async UniTaskVoid Run(MyGameObject other)
    {
        collidedObjects[hash] = other;
        await onTrigger.Execute(hash);
        collidedObjects.Remove(hash);
        hash++;
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onTrigger;
        yield return collidedObject;
    }
}
