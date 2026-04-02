using System;
using UnityEngine;

public abstract class SnapPoint : MonoBehaviour
{
    [SerializeField] CodeBlock owner;
    [SerializeField] Transform snapAnchor;
    public virtual bool IsSnappable(CodeBlock codeBlock) => codeBlock != owner;
    public CodeBlock snapped { get; private set; }
    public void Snap(CodeBlock codeBlock)
    {
        if(snapped != null || codeBlock == null || !IsSnappable(codeBlock)) return;
        snapped = codeBlock;
        snapped.snappedPoint = this;
        EditorSceneManager.Instance.scriptGrid.Remove(snapped);
        snapped.transform.SetParent(snapAnchor);
        snapped.transform.localPosition = Vector3.zero;
        OnSnappedChange();
    }
    public void Detach()
    {
        if (snapped == null) return;
        EditorSceneManager.Instance.scriptGrid.Add(snapped);
        snapped.snappedPoint = null;
        snapped = null;
        OnSnappedChange();
    }
    public virtual Vector2 GetSnapPosition() => snapAnchor.position;

    public event Action onSnappedChange;
    protected virtual void OnSnappedChange()
    {
        onSnappedChange?.Invoke();
    }
    private void OnEnable()
    {
        OnSnappedChange();
        EditorSceneManager.Instance.snapPoints.Add(this);
    }
    private void OnDisable()
    {
        EditorSceneManager.Instance.snapPoints.Remove(this);
    }
}