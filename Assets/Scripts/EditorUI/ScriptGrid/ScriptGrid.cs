using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using PrimeTween;

public class ScriptGrid : MonoBehaviour, IPointerDownHandler, IScrollHandler
{
    public float zoom { get; private set; } = 1f;
    public Vector2 panOffset { get; private set; } = Vector2.zero;

    [SerializeField] GridGraphic grid;
    [SerializeField] RectTransform anchor;
    [SerializeField] float scrollSensitivity = 1.0f, dragSensitivity = 5.0f;
    [SerializeField] BlockAddMenu blockMenu;

    public ICodeable editing { get; private set; }
    public event Action<ICodeable> onEditingChange;
    public void BindToGrid(CodeBlock block)
    {
        if (editing == null) return;
        block.transform.SetParent(anchor, true);
        block.transform.localScale = Vector3.one;
    }

    float baseSpacing;
    bool dragging;

    void Awake()
    {
        baseSpacing = grid.spacing;
        EditorSceneManager.Instance.onSelect += OnSelect;
    }
    void OnSelect(ISelectable selected)
    {
        if (selected is ICodeable codeable && codeable != editing)
        {
            if (editing != null) foreach (var block in editing.codeBlocks) block.gameObject.SetActive(false);
            editing = codeable;
            onEditingChange?.Invoke(editing);
            panOffset = editing.lastOffset;
            zoom = editing.lastZoom;
            grid.offset = panOffset;
            anchor.localPosition = panOffset;
            grid.spacing = baseSpacing * zoom;
            anchor.localScale = Vector2.one * zoom;
            grid.SetVerticesDirty();
            foreach (var block in editing.codeBlocks)
            {
                block.transform.SetParent(anchor, true);
                block.gameObject.SetActive(true);
            }
        }
    }
    const float lerpTime = 0.3f;
    public void MoveTo(Vector2 pos, bool lerp = false)
    {
        pos = grid.transform.InverseTransformPoint(pos);
        if (lerp)
        {
            Tween.Custom(panOffset, panOffset - pos, lerpTime, val => panOffset = val, Ease.OutCirc).OnUpdate<ScriptGrid>(this, (self, tween) =>
            {
                anchor.localPosition = panOffset;
                if (editing != null) editing.lastOffset = panOffset;
                grid.offset = panOffset;
                grid.SetVerticesDirty();
            });
        }
        else
        {
            panOffset -= pos;
            anchor.localPosition = panOffset;
            if (editing != null) editing.lastOffset = panOffset;
            grid.offset = panOffset;
            grid.SetVerticesDirty();
        }
    }
    void Update()
    {
        if (dragging && !Input.GetMouseButton(2)) dragging = false;
        if (dragging)
        {
            Vector2 delta = Input.mousePositionDelta;
            if (delta.sqrMagnitude > 0f)
            {
                panOffset += delta * dragSensitivity;
                anchor.localPosition = panOffset;
                if (editing != null) editing.lastOffset = panOffset;
                grid.offset = panOffset;
                grid.SetVerticesDirty();
            }
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (eventData.used) return;
        float scroll = eventData.scrollDelta.y;
        if (scroll != 0f)
        {
            float prev = zoom;
            zoom = Mathf.Clamp(zoom * (1f + scroll * scrollSensitivity), 0.1f, 10f);
            panOffset *= zoom / prev;
            anchor.localPosition = panOffset;
            if (editing != null) editing.lastOffset = panOffset;
            grid.offset = panOffset;
            grid.spacing = baseSpacing * zoom;
            anchor.localScale = Vector2.one * zoom;
            grid.SetVerticesDirty();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.Use();
            EditorSceneManager.Instance.rightClickMenu.Open(Input.mousePosition, MakeRightClickMenu());
        }
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            eventData.Use();
            dragging = true;
        }
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Add Block...",
            (ctx) => { blockMenu.Open(transform.InverseTransformPoint(ctx.position)); });
        yield return new RCMenuElement_Button(
            "Move to Center",
            (ctx) =>
            {
                MoveTo(anchor.position, true);
            });
    }
}
