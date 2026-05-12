using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyGameObject : MonoBehaviour, IParent, ICodeable, IInspectable, ISelectable
{
    public ulong uid { get; private set; }

    [HideInInspector] public bool dirty = false;
    [HideInInspector] public bool foldedInInspector = false;
    public abstract string id { get; }

    public IParent parent;
    public readonly List<MyGameObject> children = new();
    public event Action onChildrenChange;
    public List<CodeBlock> codeBlocks { get; } = new();
    public float lastZoom { get; set; } = 1.0f;
    public Vector2 lastOffset { get; set; } = Vector2.zero;

    [field:SerializeField] public Sprite icon { get; private set; }
    [SerializeField] int childrenOffset = 0;
    [SerializeField] List<CodeBlockList> availableCodeBlockLists;
    [SerializeField] List<CodeBlock> availableCodeBlocks;
    public event Action onPropertyChange;
    protected virtual void Awake()
    {
        uid = MathUtilities.GenerateRandomID();
    }
    public bool IsChildOf(IParent obj)
    {
        IParent search = parent;
        while(search != null)
        {
            if (search == obj) return true;
            search = search is MyGameObject go ? go.parent : null;
        }
        return false;
    }
    public void AddChild(MyGameObject child)
    {
        if (children.Contains(child)) return;
        if (child.parent != null && child.parent != this) child.parent.RemoveChild(child);
        child.transform.SetParent(transform, true);
        child.parent = this;
        children.Add(child);
        child.onChildrenChange += OnChildrenChange;
        onChildrenChange?.Invoke();
    }
    public void RemoveChild(MyGameObject child)
    {
        if (!children.Contains(child)) return;
        child.transform.SetParent(null, true);
        child.parent = null;
        children.Remove(child);
        child.onChildrenChange -= OnChildrenChange;
        onChildrenChange?.Invoke();
    }
    public bool HasChild(MyGameObject child) => children.Contains(child);
    public int GetChildIndex(MyGameObject obj) => children.IndexOf(obj);
    public void SetChildIndex(MyGameObject child, int index)
    {
        if (!children.Contains(child) || index < 0 || index >= children.Count) return;
        children.Remove(child); children.Insert(index, child);
        child.transform.SetSiblingIndex(index + childrenOffset);
        onChildrenChange?.Invoke();
    }
    public void SetSiblingIndex(int index) => parent.SetChildIndex(this, index);
    public int GetSiblingIndex() => parent.GetChildIndex(this);
    void OnChildrenChange() => onChildrenChange?.Invoke();
    public IEnumerable<MyGameObject> GetChildren() => children;
    public virtual IEnumerable<CodeBlock> GetAvailableBlocks()
    {
        foreach (var list in availableCodeBlockLists)
        {
            foreach (var block in list.GetBlocks())
            {
                yield return block;
            }
        }
        foreach (var block in availableCodeBlocks)
        {
            yield return block;
        }
    }
    public virtual void OnPropertyChange() => onPropertyChange?.Invoke();
    public virtual IEnumerable<ExposedElement> GetElements()
    {
        yield return new ExposedString(
            "Name",
            (self) => name,
            (self, value) => { name = value; OnPropertyChange(); });
        yield return new ExposedVector2(
            "Position",
            (self) => transform.localPosition,
            (self, value) => transform.localPosition = value);
        yield return new ExposedFloat(
            "Rotation",
            (self) => transform.localEulerAngles.z,
            (self, value) => transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value));
    }

    public readonly Dictionary<string, float> numericVariables = new();
    public virtual MyGameObjectSave Save(bool prettyPrint = true)
    {
        MyGameObjectSave save = new();
        save.id = id;
        save.uid = uid;
        save.position = transform.position;
        List<CodeBlockSave> codeBlockSaves = new();
        foreach (var block in codeBlocks)
        {
            if (block.snappedPoint != null) continue;
            var tmp = block.Save();
            tmp.position = block.transform.position;
            codeBlockSaves.Add(tmp);
        }
        save.data.SaveVector2("lastOffset", lastOffset);
        save.data.strings["CodeBlocks"] = JsonUtility.ToJson(codeBlockSaves, prettyPrint);
        foreach(var child in children)
        {
            save.children.Add(child.Save(prettyPrint));
        }
        return save;
    }
    readonly Dictionary<CodeBlock, CodeBlockSave> blockSaves = new();
    public virtual void EarlyLoad(MyGameObjectSave save)
    {
        uid = save.uid;
        if (save.data.strings.ContainsKey("CodeBlocks"))
        {
            List<CodeBlockSave> codeBlockSaves = JsonUtility.FromJson<List<CodeBlockSave>>(save.data.strings["CodeBlocks"]);
            foreach (var blockSave in codeBlockSaves)
            {
                CodeBlock blockPrefab = EditorSceneManager.Instance.IDToBlock(blockSave.id);
                if (blockPrefab != null)
                {
                    CodeBlock block = Instantiate(blockPrefab, transform);
                    block.owner = this;
                    codeBlocks.Add(block);
                    block.EarlyLoad(blockSave);
                    block.gameObject.SetActive(false);
                    blockSaves[block] = blockSave;
                }
            }
        }
    }
    public virtual void Load(MyGameObjectSave save)
    {
        codeBlocks.Clear();
        lastOffset = save.data.LoadVector2("lastOffset");
        foreach (var i in blockSaves) i.Key.Load(i.Value);
        foreach(var childSave in save.children)
        {
            MyGameObject child = Instantiate(EditorSceneManager.Instance.IDToGameObject(childSave.id), transform);
            child.Load(childSave);
            children.Add(child);
        }
    }
    public IEnumerable<MyGameObject> GetHierarchy()
    {
        yield return this;
        foreach (var child in children)
        {
            foreach (var i in child.GetHierarchy()) yield return i;
        }
    }

    public virtual void OnSelect() { }

    public virtual void OnDeselect() { }
}
[System.Serializable]
public class MyGameObjectSave
{
    public string id;
    public ulong uid;
    public Vector2 position;
    public DataUnit data = new();
    public List<MyGameObjectSave> children = new();
}
public enum MyPropertyType
{
    Number,
    
}