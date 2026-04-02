using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ScriptGrid : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public float zoom = 1f;
    public Vector2 panOffset = Vector2.zero;

    [SerializeField] GridGraphic grid;
    [SerializeField] RectTransform anchor;
    [SerializeField] float scrollSensitivity = 10.0f;

    ICodeable editing;

    [Header("Debug")]
    [SerializeField] bool debugMode = false;
    [SerializeField] List<CodeBlock> debugBlocks;
    public void Add(CodeBlock block)
    {
        if (debugMode)
        {
            block.transform.SetParent(anchor, true);
            debugBlocks.Add(block);
            return;
        }

        if (editing == null) return;
        block.transform.SetParent(anchor, true);
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
            if(editing != null) foreach (var block in editing.codeBlocks) block.gameObject.SetActive(false);
            editing = codeable;
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
        if(DeviceManager.deviceType == DeviceType.Desktop)
        {
            if (mouseOver && InputManager.PC_MiddleMouseHeld()) dragging = true;
            else if (!InputManager.PC_MiddleMouseHeld()) dragging = false;
            if (dragging)
            {
                Vector2 delta = InputManager.PC_MouseDelta();
                if (delta.sqrMagnitude > 0f)
                {
                    panOffset += delta;
                    anchor.localPosition = panOffset;
                    if (editing != null) editing.lastOffset = panOffset;
                    grid.offset = panOffset;
                    grid.SetVerticesDirty();
                }
            }
            if (mouseOver)
            {
                float scroll = InputManager.PC_ScrollDelta().y / 120f;
                if (scroll != 0f)
                {
                    zoom = Mathf.Clamp(zoom * (1f + scroll * scrollSensitivity), 0.1f, 10f);
                    grid.spacing = baseSpacing * zoom;
                    anchor.localScale = Vector2.one * zoom;
                    grid.SetVerticesDirty();
                }
            }
        }
        else if(DeviceManager.deviceType == DeviceType.Handheld)
        {
            if (dragging && !InputManager.Mobile_Touch0Hold()) dragging = false;
            if (dragging)
            {
                if (InputManager.Mobile_Touch1Hold())
                {
                    return;
                }
                Vector2 delta = InputManager.Mobile_Touch0Delta();
                if (delta.sqrMagnitude > 0f)
                {
                    panOffset += delta;
                    anchor.localPosition = panOffset;
                    if (editing != null) editing.lastOffset = panOffset;
                    grid.offset = panOffset;
                    grid.SetVerticesDirty();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => mouseOver = true;
    public void OnPointerExit(PointerEventData eventData) => mouseOver = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(DeviceManager.deviceType == DeviceType.Handheld) dragging = true;
    }
}
