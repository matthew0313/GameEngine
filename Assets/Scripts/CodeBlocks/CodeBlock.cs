using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CodeBlock : MonoBehaviour, IPointerDownHandler
{
    public ulong uid { get; private set; }
    public ICodeable owner { get; private set; }

    [HideInInspector] public SnapPoint snappedPoint;
    [field:SerializeField] public string blockID { get; private set; }
    [field:SerializeField] public Color blockColor { get; private set; }
    public abstract CodeBlockCategory category { get; }

    public virtual void Set(ICodeable owner)
    {
        this.owner = owner;
    }
    public virtual bool IsAddable(ICodeable codeable) => true;

    bool dragging = false;
    Vector2 dragOffset;
    public const float snapDistance = 20.0f;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            dragging = true;
            dragOffset = (Vector2)transform.position - eventData.position;
            transform.SetAsLastSibling();
            eventData.Use();
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            EditorSceneManager.Instance.rightClickMenu.Open(eventData.position, MakeRightClickMenu());
            eventData.Use();
        }
    }
    public virtual void Delete()
    {
        if (snappedPoint != null) snappedPoint.Detach();
        foreach (var snapPoint in GetSnapPoints())
        {
            snapPoint.enabled = false;
            snapPoint.Clear();
        }
        if (owner != null) owner.codeBlocks.Remove(this);
        Destroy(gameObject);
    }
    protected virtual IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Copy",
            ctx => {
                EditorSceneManager.Instance.CopyBlock(this);
            });
        yield return new RCMenuElement_Button(
            "Delete",
            ctx => { Delete(); });
    }
    protected virtual void Awake()
    {
        uid = MathUtilities.GenerateRandomID();
    }
    protected virtual void Update()
    {
        if (dragging) HandleDrag();
    }
    SnapPoint highlighted = null;
    void HandleDrag()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 pos = (Vector2)Input.mousePosition + dragOffset;
            if (snappedPoint != null)
            {
                if (Vector2.Distance(snappedPoint.GetSnapPosition(), pos) > snapDistance)
                {
                    snappedPoint.Detach();
                }
            }
            else
            {
                transform.position = pos;
                Vector2 snapPos = pos;
                if (this is PropertyCodeBlock) snapPos -= dragOffset;
                var snapPoints = EditorSceneManager.Instance.snapPoints.ToList();
                RemoveSnapPoints(snapPoints);
                snapPoints.Sort((a, b) =>
                {
                    if (a.IsSnappable(this))
                    {
                        if (b.IsSnappable(this))
                        {
                            float distA = Vector2.Distance(a.GetSnapPosition(), snapPos);
                            float distB = Vector2.Distance(b.GetSnapPosition(), snapPos);
                            return distA.CompareTo(distB);
                        }
                        else return -1;
                    }
                    else
                    {
                        if (b.IsSnappable(this)) return 1;
                        else return 0;
                    }
                });
                var tmp = (snapPoints.Count > 0 && snapPoints[0].IsSnappable(this) && Vector2.Distance(snapPoints[0].GetSnapPosition(), snapPos) <= snapDistance) ? snapPoints[0] : null;
                if(highlighted != tmp)
                {
                    if (highlighted != null) highlighted.UnHighlight();
                    highlighted = tmp;
                    if (highlighted != null) highlighted.Highlight();
                }
            }
        }
        else
        {
            if (snappedPoint == null)
            {
                if (highlighted != null)
                {
                    highlighted.UnHighlight();
                    highlighted.Snap(this);
                    highlighted = null;
                }
            }
            dragging = false;
        }
    }
    void RemoveSnapPoints(List<SnapPoint> snapPoints)
    {
        if (snapPoints == null) return;
        foreach(var i in GetSnapPoints())
        {
            if (snapPoints.Contains(i)) snapPoints.Remove(i);
            if (i.snapped != null) i.snapped.RemoveSnapPoints(snapPoints);
        }
    }
    protected virtual IEnumerable<SnapPoint> GetSnapPoints() { yield break; }
    public virtual CodeBlockSave Save()
    {
        CodeBlockSave save = new();
        save.id = blockID;
        save.uid = uid;
        save.position = transform.localPosition;
        foreach (var snapPoint in GetSnapPoints()) save.snapPoints.Add(snapPoint.Save());
        return save;
    }
    public virtual void EarlyLoad(CodeBlockSave save, bool resetUID = false)
    {
        uid = resetUID ? MathUtilities.GenerateRandomID() : save.uid;
        int index = 0;
        foreach (var snapPoint in GetSnapPoints())
        {
            if (index >= save.snapPoints.Count) break;
            snapPoint.EarlyLoad(save.snapPoints[index++]);
        }
    }
    public virtual void Load(CodeBlockSave save)
    {
        transform.localPosition = save.position;
        int index = 0;
        foreach (var snapPoint in GetSnapPoints())
        {
            if (index >= save.snapPoints.Count) break;
            snapPoint.Load(save.snapPoints[index++]);
        }
    }
}
[System.Serializable]
public enum CodeBlockCategory
{
    Starter = 0,
    Movement = 1,
    Logic = 2,
    Calculation = 3,
    Condition = 4,
    Other = 5,
    Debug = 6
}
public class CodeBlockSave
{
    public string id;
    public ulong uid;
    public Vector2 position;
    public DataUnit data = new();
    public List<SnapPointSave> snapPoints = new();
}