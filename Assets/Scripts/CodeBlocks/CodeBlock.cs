using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CodeBlock : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector] public SnapPoint snappedPoint;
    [HideInInspector] public MyGameObject owner;
    public abstract string id { get; }
    public virtual CodeBlockSave Save()
    {
        CodeBlockSave save = new();
        save.id = id;
        return save;
    }
    public virtual void Load(CodeBlockSave save) { }

    bool dragging = false;
    Vector2 dragOffset;
    public const float snapDistance = 20.0f;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        dragging = true;
        dragOffset = (Vector2)transform.position - eventData.position;
        transform.SetAsLastSibling();
        eventData.Use();
    }
    protected virtual void Update()
    {
        if (dragging) HandleDrag();
    }
    void HandleDrag()
    {
        if (InputManager.LeftClickHoldInput())
        {
            Vector2 pos = InputManager.MousePosInput() + dragOffset;
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
public class CodeBlockSave
{
    public string id;
    public Vector2 position;
    public DataUnit data = new();   
}