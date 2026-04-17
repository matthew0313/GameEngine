using System;
using UnityEngine;

public abstract class SnapPoint : MonoBehaviour
{
    [SerializeField] protected CodeBlock ownerBlock;
    [SerializeField] protected Transform snapAnchor;
    public virtual bool IsSnappable(CodeBlock codeBlock) => codeBlock != ownerBlock;
    public CodeBlock snapped { get; protected set; }
    public virtual void Snap(CodeBlock codeBlock)
    {
        if(codeBlock == null || !IsSnappable(codeBlock)) return;
        if (codeBlock.snappedPoint != null) codeBlock.snappedPoint.Detach();
        if (snapped != null)
        {
            snapped.transform.position += Vector3.right * 30.0f;
            Detach();
        }
        snapped = codeBlock;
        snapped.snappedPoint = this;
        snapped.transform.SetParent(snapAnchor);
        snapped.transform.localPosition = Vector3.zero;
        OnSnappedChange();
    }
    public void Detach()
    {
        if (snapped == null) return;
        EditorSceneManager.Instance.scriptGrid.BindToGrid(snapped);
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