using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightClickMenuButton : RightClickMenuElement, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] TMP_Text text;
    [SerializeField] Image image;
    [SerializeField] Color hoverColor, normalColor;
    RightClickMenu menu;
    public void Init(RightClickMenu menu)
    {
        this.menu = menu;
    }
    Action onClick;
    public void Set(RCMenuElement_Button content)
    {
        text.text = content.name;
        onClick = content.onClick;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.Use();
            menu.Close();
            onClick?.Invoke();
        }
    }
}