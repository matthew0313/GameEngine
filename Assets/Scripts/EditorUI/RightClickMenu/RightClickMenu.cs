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
    public RCMenuContext context => new()
    {
        position = pos
    };
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
        foldoutPool.onRelease = item => { item.gameObject.SetActive(false); item.Clear(); };
    }
    bool skipFrame = false;
    bool open = false;
    Vector2 pos;
    public void Open(Vector2 pos, IEnumerable<RCMenuElement> elements)
    {
        skipFrame = true;
        if (!open)
        {
            open = true;
            gameObject.SetActive(true);
        }
        this.pos = pos;
        pos = area.InverseTransformPoint(pos);
        rectTransform.anchoredPosition = pos;
        foreach(var i in this.elements)
        {
            if (i is RightClickMenuButton button) buttonPool.ReleaseObject(button);
            if (i is RightClickMenuFoldout foldout) foldoutPool.ReleaseObject(foldout);
        }
        this.elements.Clear();
        foreach(var i in elements)
        {
            if(i is RCMenuElement_Button button)
            {
                var tmp = buttonPool.GetObject(elementAnchor);
                tmp.transform.SetAsLastSibling();
                tmp.Set(button);
                this.elements.Add(tmp);
            }
            if(i is RCMenuElement_Foldout foldout)
            {
                var tmp = foldoutPool.GetObject(elementAnchor);
                tmp.transform.SetAsLastSibling();
                tmp.Set(foldout);
                this.elements.Add(tmp);
            }
        }
    }
    public void Close()
    {
        if (!open) return;
        Debug.Log("Closes");
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
        if (skipFrame)
        {
            skipFrame = false; return;
        }
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
        if (open && (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))) Close();
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
public struct RCMenuContext
{
    public Vector2 position;
}
public class RCMenuElement_Button : RCMenuElement
{
    public readonly Action<RCMenuContext> onClick;
    public RCMenuElement_Button(string name, Action<RCMenuContext> onClick) : base(name)
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