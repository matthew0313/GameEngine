using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class BlockAddMenu : MonoBehaviour
{
    [SerializeField] ScriptGrid scriptGrid;
    [SerializeField] RectTransform rectTransform, area;
    [SerializeField] TMP_InputField searchBar;
    [SerializeField] GameObject notSelectedIndicator;
    [SerializeField] Transform elementAnchor;
    [SerializeField] BlockAddMenuCategory categoryPrefab;
    [SerializeField] BlockAddMenuElement elementPrefab;
    Pooler<BlockAddMenuElement> elementPool;
    readonly Dictionary<CodeBlockCategory, BlockAddMenuCategory> categories = new();
    private void OnEnable()
    {
        elementPool ??= new(() =>
        {
            var tmp = Instantiate(elementPrefab);
            tmp.Init(this);
            return tmp;
        });
        scriptGrid.onEditingChange += OnEditingChange;
        searchBar.onEndEdit.AddListener(OnEndEdit);
        Refresh();
    }
    private void OnDisable()
    {
        scriptGrid.onEditingChange -= OnEditingChange;
        searchBar.onEndEdit.RemoveListener(OnEndEdit);
    }
    void OnEditingChange(ICodeable codeable) => Refresh();
    void OnEndEdit(string content) => Refresh();
    private void Update()
    {
        if(open && Input.GetMouseButtonDown(0))
        {
            bool hitSelf = false;
            foreach(var hit in EditorSceneManager.Instance.RaycastUI(Input.mousePosition))
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
    void Refresh()
    {
        ICodeable codeable = scriptGrid.editing;
        if (codeable == null)
        {
            notSelectedIndicator.SetActive(true);
            foreach (var item in categories) item.Value.gameObject.SetActive(false);
            return;
        }
        else notSelectedIndicator.SetActive(false);

        foreach (var item in categories)
        {
            item.Value.Clear();
            item.Value.gameObject.SetActive(false);
        }
        foreach(var block in GetAvailableBlocksFiltered())
        {
            if (!categories.ContainsKey(block.category))
            {
                var tmp = Instantiate(categoryPrefab, elementAnchor);
                tmp.Init(this, elementPool); tmp.Set(block.category);
                categories.Add(block.category, tmp);
            }
            categories[block.category].gameObject.SetActive(true);
            categories[block.category].Add(block);
        }
    }
    public IEnumerable<CodeBlock> GetAvailableBlocksFiltered()
    {
        ICodeable codeable = scriptGrid.editing;
        if (codeable == null) yield break;
        foreach (var block in codeable.GetAvailableBlocks())
        {
            if (!block.blockID.StartsWith(searchBar.text)) continue;
            yield return block;
        }
    }
}