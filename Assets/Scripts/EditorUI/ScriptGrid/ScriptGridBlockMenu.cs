using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ScriptGridBlockMenu : MonoBehaviour
{
    [SerializeField] ScriptGrid scriptGrid;
    [SerializeField] RectTransform rectTransform, area;
    [SerializeField] Transform elementAnchor;
    [SerializeField] ScriptGridBlockMenuElement elementPrefab;
    readonly List<ScriptGridBlockMenuElement> elements = new();
    private void OnEnable()
    {
        scriptGrid.onEditingChange += Refresh;
        Refresh(scriptGrid.editing);
    }
    private void OnDisable()
    {
        scriptGrid.onEditingChange -= Refresh;
    }
    private void Update()
    {
        if(open && Input.GetMouseButtonDown(0))
        {
            bool hitSelf = false;
            List<RaycastResult> list = EditorSceneManager.Instance.RaycastUI(Input.mousePosition);
            foreach(var hit in list)
            {
                if (hit.gameObject == gameObject || hit.gameObject.transform.IsChildOf(transform))
                {
                    hitSelf = true; break;
                }
            }
            if (!hitSelf) Close();
        }
    }
    public bool open { get; private set; } = false;
    public void Open(Vector2 pos)
    {
        if (open) return;
        open = true;
        Vector2 center = area.rect.center;
        rectTransform.pivot = new Vector2(pos.x < center.x ? 0 : 1, pos.y < center.y ? 0 : 1);
        rectTransform.anchoredPosition = pos;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        if (!open) return;
        open = false;
        gameObject.SetActive(false);
    }
    public void AddBlock(CodeBlock codeBlock)
    {
        scriptGrid.Add(Instantiate(codeBlock, rectTransform.position, Quaternion.identity));
        Close();
    }
    void Refresh(ICodeable codeable)
    {
        if(codeable == null)
        {
            foreach (var element in elements) element.gameObject.SetActive(false);
            return;
        }
        int i = 0;
        foreach(var block in codeable.GetAvailableBlocks())
        {
            if(elements.Count <= i)
            {
                var tmp = Instantiate(elementPrefab, elementAnchor);
                tmp.Init(this);
                elements.Add(tmp);
            }
            elements[i].Set(block);
            elements[i].gameObject.SetActive(true); i++;
        }
        for (; i < elements.Count; i++) elements[i].gameObject.SetActive(false);
    }
}