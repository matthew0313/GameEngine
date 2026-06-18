using UnityEngine;

/// <summary>Base for numeric getter Rigidbody blocks (Movement category). Addable only to a
/// Rigidbody object or a PrefabAsset whose origin is one.</summary>
public abstract class RigidbodyNumericCodeBlock : NumericCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Movement;
    [SerializeField] protected RectTransform rectTransform;

    public override bool IsAddable(ICodeable codeable) =>
        base.IsAddable(codeable) &&
        (codeable is MyGameObject_Rigidbody ||
        (codeable is PrefabAsset prefab && prefab.prefabOrigin is MyGameObject_Rigidbody));
    public override float GetNumber(ulong hash) => owner is MyGameObject_Rigidbody rb ? GetValue(rb) : 0f;
    protected abstract float GetValue(MyGameObject_Rigidbody rb);
    public override float GetWidth() => rectTransform.rect.width;
}
