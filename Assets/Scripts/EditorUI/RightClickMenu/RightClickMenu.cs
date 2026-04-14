using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenu : MonoBehaviour
{
    [SerializeField] RectTransform area, rectTransform;
    [SerializeField] Transform elementAnchor;
    [SerializeField] RightClickMenuButton buttonElement;
    [SerializeField] RightClickMenuFoldout foldoutElement;
    readonly List<RightClickMenuElement> elements = new();
    Pooler<RightClickMenuButton> buttonPool;
    Pooler<RightClickMenuFoldout> foldoutPool;
    bool initialized = false;
    void Init()
    {
        buttonPool = new(() =>
        {
            var tmp = Instantiate(buttonElement); tmp.Init(this);
            return tmp;
        });
        foldoutPool = new(() =>
        {
            var tmp = Instantiate(foldoutElement); tmp.Init(this, buttonPool, foldoutPool);
            return tmp;
        });
    }
    bool open = false;
    public void Open(Vector2 pos, IEnumerable<RCMenuElement> elements)
    {
        if (!open)
        {
            open = true;
            gameObject.SetActive(true);
        }
        Vector2 center = area.rect.center;
        rectTransform.pivot = new Vector2(pos.x < center.x ? 0 : 1, pos.y < center.y ? 0 : 1);
        rectTransform.anchoredPosition = pos;
        foreach(var i in this.elements)
        {
            if (i is RightClickMenuButton button) buttonPool.ReleaseObject(button);
            if (i is RightClickMenuFoldout foldout) foldoutPool.ReleaseObject(foldout);
        }
        foreach(var i in elements)
        {
            if(i is RCMenuElement_Button button)
            {
                var tmp = buttonPool.GetObject(elementAnchor);
                tmp.Set(button);
                this.elements.Add(tmp);
            }
        }
    }
    public void Close()
    {
        if (!open) return;
        open = false;
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if (!initialized)
        {
            initialized = true;
            Init();
        }
    }
    private void Update()
    {
        if (open && Input.GetMouseButtonDown(0))
        {
            bool hitSelf = false;
            foreach (var hit in EditorSceneManager.Instance.RaycastUI(Input.mousePosition))
            {
                if (hit.gameObject == gameObject || hit.gameObject.transform.IsChildOf(transform))
                {
                    hitSelf = true; break;
                }
            }
            if (!hitSelf) Close();
        }
        if (open && (Input.GetMouseButton(1) || Input.GetMouseButtonDown(2))) Close();
    }
}
public abstract class RCMenuElement
{
    public readonly string name;
    public RCMenuElement(string name)
    {
        this.name = name;
    }
}
public class RCMenuElement_Button : RCMenuElement
{
    public readonly Action onClick;
    public RCMenuElement_Button(string name, Action onClick) : base(name)
    {
        this.onClick = onClick;
    }
}
public class RCMenuElement_Foldout : RCMenuElement
{
    public readonly IEnumerable<RCMenuElement> elements;
    public RCMenuElement_Foldout(string name, IEnumerable<RCMenuElement> elements) : base(name)
    {
        this.elements = elements;
    }
}