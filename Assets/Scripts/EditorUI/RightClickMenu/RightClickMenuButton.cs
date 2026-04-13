using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenuButton : RightClickMenuElement
{
    [SerializeField] TMP_Text text;
    [SerializeField] Button button;
    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }
    Action onClick;
    void OnClick() => onClick?.Invoke();
    public void Set(RCMenuElement_Button content)
    {
        text.text = content.name;
        onClick = content.onClick;
    }
}