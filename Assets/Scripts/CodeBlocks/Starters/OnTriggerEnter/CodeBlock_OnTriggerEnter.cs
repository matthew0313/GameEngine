using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_OnTriggerEnter : CodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Starter;
    [SerializeField] ExecutableSnapPoint onTriggerEnter;
    [SerializeField] SnapPoint_CollidedObject collidedObject;
    public ExecutableSnapPoint onFinish => onTriggerEnter;
    public override bool IsAddable(ICodeable codeable)
    {
        return base.IsAddable(codeable) &&
            (codeable is MyGameObject_Rigidbody ||
            codeable is PrefabAsset prefab && prefab.prefabOrigin is MyGameObject_Rigidbody);
    }

    readonly Dictionary<ulong, MyGameObject> collidedObjects = new();
    public MyGameObject GetCollidedObject(ulong hash)
    {
        if (collidedObjects.TryGetValue(hash, out var obj)) return obj;
        return null;
    }

    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        if (owner is MyGameObject_Rigidbody rb) rb.onTriggerEnter += OnTriggerEnter;
    }
    ulong testHash = 0, hash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Execute",
            ctx =>
            {
                onTriggerEnter.Execute(testHash++);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    private void OnTriggerEnter(Collider2D collider)
    {
        Run(collider != null ? collider.GetComponentInParent<MyGameObject>() : null).Forget();
    }
    private async UniTaskVoid Run(MyGameObject other)
    {
        collidedObjects[hash] = other;
        await onTriggerEnter.Execute(hash);
        collidedObjects.Remove(hash);
        hash++;
    }
    public override void Delete()
    {
        if (owner is MyGameObject_Rigidbody rb) rb.onTriggerEnter -= OnTriggerEnter;
        base.Delete();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onTriggerEnter;
        yield return collidedObject;
    }
}
