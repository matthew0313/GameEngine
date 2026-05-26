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
    [field:SerializeField] public CodeBlockCategory category { get; private set; }
    public virtual bool addable => true;

    public virtual void Set(ICodeable owner)
    {
        this.owner = owner;
    }
    public virtual CodeBlockSave Save()
    {
        CodeBlockSave save = new();
        save.id = blockID;
        save.uid = uid;
        save.position = transform.position;
        return save;
    }
    public virtual void EarlyLoad(CodeBlockSave save)
    {
        uid = save.uid;
    }
    public virtual void Load(CodeBlockSave save)
    {
        transform.position = save.position;
    }

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
    public void Delete()
    {
        if (owner != null) owner.codeBlocks.Remove(this);
        Destroy(gameObject);
    }
    protected virtual IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
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
            else transform.position = pos;
        }
        else
        {
            if (snappedPoint == null)
            {
                var snapPoints = EditorSceneManager.Instance.snapPoints.ToList();
                snapPoints.Sort((a, b) =>
                {
                    if (a.IsSnappable(this))
                    {
                        if (b.IsSnappable(this))
                        {
                            float distA = Vector2.Distance(a.GetSnapPosition(), transform.position);
                            float distB = Vector2.Distance(b.GetSnapPosition(), transform.position);
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
                if (snapPoints.Count > 0 && snapPoints[0].IsSnappable(this) && Vector2.Distance(snapPoints[0].GetSnapPosition(), transform.position) <= snapDistance)
                {
                    snapPoints[0].Snap(this);
                }
            }
            dragging = false;
        }
    }
}
[System.Serializable]
public enum CodeBlockCategory
{
    Movement,
    Logic,
    Calculation,
    Debug,
    Other
}
[System.Serializable]
public class CodeBlockSave
{
    public string id;
    public ulong uid;
    public Vector2 position;
    public DataUnit data = new();
    public SerializableDictionary<string, SnapPointSave> snapPoints = new();
}