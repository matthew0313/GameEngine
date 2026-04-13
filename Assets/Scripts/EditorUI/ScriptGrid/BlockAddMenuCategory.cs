using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockAddMenuCategory : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Transform elementAnchor;
    Pooler<BlockAddMenuElement> elementPool;
    BlockAddMenu menu;
    readonly List<BlockAddMenuElement> elements = new();
    public void Init(BlockAddMenu menu, Pooler<BlockAddMenuElement> elementPool)
    {
        this.menu = menu;
        this.elementPool = elementPool;
    }
    public void Set(CodeBlockCategory category)
    {
        text.text = category.ToString();
    }
    public void Clear()
    {
        foreach (var i in elements) elementPool.ReleaseObject(i);
        elements.Clear();
    }
    public void Add(CodeBlock block)
    {
        var tmp = elementPool.GetObject(elementAnchor);
        tmp.Set(block);
        elements.Add(tmp);
    }
}