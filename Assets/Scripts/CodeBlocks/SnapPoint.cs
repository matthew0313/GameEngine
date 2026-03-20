using System;
using UnityEngine;

public abstract class SnapPoint : MonoBehaviour
{
    [SerializeField] Transform snapAnchor;
    public abstract bool IsSnappable(CodeBlock codeBlock);
    public CodeBlock snapped { get; private set; }
    public void Snap(CodeBlock codeBlock)
    {
        if(snapped != null || codeBlock == null || !IsSnappable(codeBlock)) return;
        snapped = codeBlock;
        snapped.snappedPoint = this;
        snapped.transform.SetParent(snapAnchor);
        snapped.transform.localPosition = Vector3.zero;
        OnSnappedChange();
    }
    public void Detach()
    {
        if (snapped == null) return;
        snapped.transform.SetParent(EditorSceneManager.Instance.codeTab.blockAnchor, true);
        snapped.snappedPoint = null;
        snapped = null;
        OnSnappedChange();
    }

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