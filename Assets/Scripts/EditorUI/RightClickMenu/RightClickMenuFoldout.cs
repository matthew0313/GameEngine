using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenuFoldout : RightClickMenuElement
{
    [SerializeField] TMP_Text text;
    [SerializeField] Button button;
    [SerializeField] Transform elementAnchor;
    RightClickMenu menu;
    Pooler<RightClickMenuButton> buttonPool;
    Pooler<RightClickMenuFoldout> foldoutPool;
    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }
    void OnClick()
    {
        
    }
    public void Init(RightClickMenu menu, Pooler<RightClickMenuButton> buttonPool, Pooler<RightClickMenuFoldout> foldoutPool)
    {

    }
    public void Set(RCMenuElement_Foldout content)
    {
        text.text = content.name;
    }
}