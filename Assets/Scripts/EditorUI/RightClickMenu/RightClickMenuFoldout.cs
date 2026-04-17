using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightClickMenuFoldout : RightClickMenuElement, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] TMP_Text text;
    [SerializeField] Image image;
    [SerializeField] Color hoverColor, normalColor;
    [SerializeField] Transform elementAnchor;
    readonly List<RightClickMenuElement> elements = new();
    RightClickMenu menu;
    Pooler<RightClickMenuButton> buttonPool;
    Pooler<RightClickMenuFoldout> foldoutPool;
    public void Init(RightClickMenu menu, Pooler<RightClickMenuButton> buttonPool, Pooler<RightClickMenuFoldout> foldoutPool)
    {
        this.menu = menu;
        this.buttonPool = buttonPool;
        this.foldoutPool = foldoutPool;
    }
    public void Clear()
    {
        foreach (var i in this.elements)
        {
            if (i is RightClickMenuButton button) buttonPool.ReleaseObject(button);
            if (i is RightClickMenuFoldout foldout) foldoutPool.ReleaseObject(foldout);
        }
        this.elements.Clear();
    }
    public void Set(RCMenuElement_Foldout content)
    {
        text.text = content.name;
        foreach (var i in this.elements)
        {
            if (i is RightClickMenuButton button) buttonPool.ReleaseObject(button);
            if (i is RightClickMenuFoldout foldout) foldoutPool.ReleaseObject(foldout);
        }
        this.elements.Clear();
        IEnumerable<RCMenuElement> elements = content.elements;
        foreach (var i in elements)
        {
            if (i is RCMenuElement_Button button)
            {
                var tmp = buttonPool.GetObject(elementAnchor);
                tmp.Set(button);
                this.elements.Add(tmp);
            }
            if (i is RCMenuElement_Foldout foldout)
            {
                var tmp = foldoutPool.GetObject(elementAnchor);
                tmp.Set(foldout);
                this.elements.Add(tmp);
            }
        }
        image.color = normalColor;
        if (open) Close();
    }
    bool open = false;
    void Open()
    {
        if (open) return;
        open = true;
        elementAnchor.gameObject.SetActive(true);
    }
    void Close()
    {
        if (!open) return;
        open = false;
        elementAnchor.gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
        if (!open) Open();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
        if (open) Close();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.Use();
            if (!open) Open();
        }
    }
}