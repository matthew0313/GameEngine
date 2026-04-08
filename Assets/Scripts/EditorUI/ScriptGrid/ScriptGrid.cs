using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class ScriptGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float zoom { get; private set; } = 1f;
    public Vector2 panOffset { get; private set; } = Vector2.zero;

    [SerializeField] GridGraphic grid;
    [SerializeField] RectTransform anchor;
    [SerializeField] float scrollSensitivity = 1.0f, dragSensitivity = 5.0f;
    [SerializeField] ScriptGridBlockMenu blockMenu;

    public ICodeable editing { get; private set; }
    public event Action<ICodeable> onEditingChange;

    [Header("Debug")]
    [SerializeField] bool debugMode = false;
    [SerializeField] List<CodeBlock> debugBlocks;
    public void Add(CodeBlock block, bool center = false)
    {
        if (debugMode && editing == null)
        {
            block.transform.SetParent(anchor, true);
            debugBlocks.Add(block);
            return;
        }

        if (editing == null) return;
        block.transform.SetParent(anchor, true);
        if (center) block.transform.localPosition = -panOffset;
        editing.codeBlocks.Add(block);
    }
    public void Remove(CodeBlock block)
    {
        if (debugMode)
        {
            debugBlocks.Remove(block);
            return;
        }

        if (editing == null) return;
        editing.codeBlocks.Remove(block);
    }

    float baseSpacing;
    bool mouseOver;
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
            if(debugMode) foreach(var block in debugBlocks) block.gameObject.SetActive(false);
            if (editing != null) foreach (var block in editing.codeBlocks) block.gameObject.SetActive(false);
            editing = codeable;
            onEditingChange?.Invoke(editing);
            panOffset = editing.lastOffset;
            grid.offset = panOffset;
            anchor.localPosition = panOffset;
            foreach (var block in editing.codeBlocks)
            {
                block.transform.SetParent(anchor, true);
                block.gameObject.SetActive(true);
            }
        }
    }
    void Update()
    {
        if (mouseOver && Input.GetMouseButton(2)) dragging = true;
        else if (!Input.GetMouseButton(2)) dragging = false;
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
        if (mouseOver)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0f)
            {
                zoom = Mathf.Clamp(zoom * (1f + scroll * scrollSensitivity), 0.1f, 10f);
                grid.spacing = baseSpacing * zoom;
                anchor.localScale = Vector2.one * zoom;
                grid.SetVerticesDirty();
            }
            if (Input.GetMouseButtonDown(1) && !blockMenu.open) blockMenu.Open(Input.mousePosition);
        }
    }
    public void OnPointerEnter(PointerEventData eventData) => mouseOver = true;
    public void OnPointerExit(PointerEventData eventData) => mouseOver = false;
}
