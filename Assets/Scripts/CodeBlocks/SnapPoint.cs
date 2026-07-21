using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class SnapPoint : MonoBehaviour
{
    [SerializeField] protected CodeBlock ownerBlock;
    [SerializeField] protected Transform snapAnchor;
    [SerializeField] GameObject highlight;
    [field:SerializeField] public string snapPointName { get; private set; }
    public virtual bool IsSnappable(CodeBlock codeBlock) => codeBlock != ownerBlock;
    public CodeBlock snapped { get; protected set; }
    public virtual void Snap(CodeBlock codeBlock)
    {
        if (codeBlock == null) return;
        if (codeBlock.snappedPoint != null) codeBlock.snappedPoint.Detach();
        if (snapped != null)
        {
            snapped.transform.position += Vector3.right * 30.0f;
            Detach();
        }
        snapped = codeBlock;
        snapped.snappedPoint = this;
        snapped.transform.SetParent(snapAnchor, true);
        snapped.transform.localScale = Vector3.one;
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
    protected virtual void Start()
    {
        OnSnappedChange();
    }
    protected virtual void OnEnable()
    {
        EditorSceneManager.Instance.snapPoints.Add(this);
    }
    protected virtual void OnDisable()
    {
        EditorSceneManager.Instance.snapPoints.Remove(this);
    }
    public virtual void Highlight()
    {
        if(highlight != null) highlight.SetActive(true);
    }
    public virtual void UnHighlight()
    {
        if(highlight != null) highlight.SetActive(false);
    }
    public virtual void Clear()
    {
        var tmp = snapped;
        Detach(); tmp?.Delete();
    }
    public virtual SnapPointSave Save()
    {
        SnapPointSave save = new();
        if (snapped != null) save.snapped = snapped.Save();
        return save;
    }
    CodeBlock snapQueued = null;
    public virtual void EarlyLoad(SnapPointSave save, bool resetUID = false)
    {
        if (save.snapped != null && save.snapped.id != string.Empty)
        {
            var block = Instantiate(EditorSceneManager.Instance.IDToBlockPrefab(save.snapped.id));
            block.Set(ownerBlock.owner);
            block.EarlyLoad(save.snapped, resetUID);
            snapQueued = block;
        }
    }
    public virtual void Load(SnapPointSave save)
    {
        if(snapQueued != null)
        {
            snapQueued.Load(save.snapped);
            Snap(snapQueued);
            snapQueued = null;
        }
    }
}
public class SnapPointSave
{
    public CodeBlockSave snapped;
    public DataUnit data = new();
}