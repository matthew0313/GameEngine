using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MyGameObject : MonoBehaviour, IParent, ICodeable, IInspectable, ISelectable
{
    public ulong uid { get; private set; }

    [HideInInspector] public bool dirty = false;
    [HideInInspector] public bool foldedInInspector = false;
    public abstract MyGameObjectType type { get; }

    public IParent parent;
    public readonly List<MyGameObject> children = new();
    public event Action onChildrenChange;
    public List<CodeBlock> codeBlocks { get; } = new();
    public float lastZoom { get; set; } = 1.0f;
    public Vector2 lastOffset { get; set; } = Vector2.zero;

    [field:SerializeField] public Sprite icon { get; private set; }
    [SerializeField] int childrenOffset = 0;
    public event Action onDisplayChange;
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
    public virtual void OnDisplayChange() => onDisplayChange?.Invoke();
    public virtual IEnumerable<ExposedElement> GetElements()
    {
        yield return new ExposedString(
            "Name",
            () => name,
            (value) => { name = value; OnDisplayChange(); });
        yield return new ExposedVector2(
            "Position",
            () => transform.localPosition,
            (value) => transform.localPosition = value);
        yield return new ExposedNumber(
            "Rotation",
            () => transform.localEulerAngles.z,
            (value) => transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value));
        yield return new ExposedVector2(
            "Scale",
            () => transform.localScale,
            (value) => transform.localScale = new Vector3(value.x, value.y, 1.0f));
    }
    public Action onInspectorChange { get; set; }

    public readonly Dictionary<string, MyVariable> variables = new();
    public readonly List<CodeBlock_Function> functions = new();
    public event Action onAwake, onStart, onUpdate;
    public virtual void OnAwake()
    {
        onAwake?.Invoke();
    }
    public virtual void OnStart()
    {
        onStart?.Invoke();
    }
    public virtual void OnUpdate()
    {
        onUpdate?.Invoke();
    }
    public virtual MyGameObjectSave Save(bool prettyPrint = true)
    {
        MyGameObjectSave save = new();
        save.name = gameObject.name;
        save.type = type;
        save.uid = uid;
        save.position = transform.localPosition;
        save.rotation = transform.localRotation.z;
        save.scale = transform.localScale;
        save.lastOffset = lastOffset;
        List<CodeBlockSave> codeBlockSaves = new();
        foreach (var block in codeBlocks)
        {
            if (block.snappedPoint != null) continue;
            var tmp = block.Save();
            tmp.position = block.transform.position;
            save.codeBlocks.Add(tmp);
        }
        foreach(var child in children)
        {
            save.children.Add(child.Save(prettyPrint));
        }
        return save;
    }
    readonly Dictionary<CodeBlock, CodeBlockSave> blockSaves = new();
    readonly Dictionary<MyGameObject, MyGameObjectSave> childSaves = new();
    public virtual void EarlyLoad(MyGameObjectSave save, bool resetUID = false)
    {
        uid = resetUID ? MathUtilities.GenerateRandomID() : save.uid;
        foreach (var blockSave in save.codeBlocks)
        {
            CodeBlock blockPrefab = EditorSceneManager.Instance.IDToBlockPrefab(blockSave.id);
            if (blockPrefab != null)
            {
                CodeBlock block = Instantiate(blockPrefab, transform);
                block.Set(this);
                EditorSceneManager.Instance.scriptGrid.BindToGrid(block);
                codeBlocks.Add(block);
                block.EarlyLoad(blockSave, resetUID);
                block.gameObject.SetActive(false);
                blockSaves[block] = blockSave;
                Debug.Log(blockSave.id);
            }
        }
        foreach (var childSave in save.children)
        {
            MyGameObject child = Instantiate(EditorSceneManager.Instance.TypeToObjectPrefab(childSave.type), transform);
            child.EarlyLoad(childSave, resetUID);
            children.Add(child);
            childSaves[child] = childSave;
        }
    }
    public virtual void Load(MyGameObjectSave save)
    {
        gameObject.name = save.name;
        transform.localPosition = save.position;
        transform.localRotation = Quaternion.Euler(0, 0, save.rotation);
        transform.localScale = save.scale;
        lastOffset = save.lastOffset;
        foreach (var i in blockSaves) i.Key.Load(i.Value);
        foreach (var i in childSaves) i.Key.Load(i.Value);
        OnDisplayChange();
    }
    public IEnumerable<MyGameObject> GetHierarchy()
    {
        yield return this;
        foreach (var child in children)
        {
            foreach (var i in child.GetHierarchy()) yield return i;
        }
    }
    public event Action onDelete;
    public void Delete()
    {
        while(children.Count > 0) children[0].Delete();
        if (parent != null) parent.RemoveChild(this);
        if (EditorSceneManager.Instance.selected == this) EditorSceneManager.Instance.Select(null);
        while(codeBlocks.Count > 0)
        {
            Destroy(codeBlocks[0].gameObject);
            codeBlocks.RemoveAt(0);
        }
        onDelete?.Invoke();
        Destroy(gameObject);
    }

    public virtual void OnSelect() { }

    public virtual void OnDeselect() { }
}
[System.Serializable]
public enum MyGameObjectType
{
    Point,
    Sprite,
    Camera,
    Canvas,
    Rigidbody,
    Image,
    Screen
}
[System.Serializable]
public class MyGameObjectSave
{
    public string name;
    public MyGameObjectType type;
    public ulong uid;
    public Vector2 position;
    public float rotation;
    public Vector2 scale;
    public Vector2 lastOffset;
    public DataUnit data = new();
    public List<CodeBlockSave> codeBlocks = new();
    public List<MyGameObjectSave> children = new();
}
public class MyVariable
{
    public float number = 0.0f;
    public bool condition = false;
    public string str = null;
    public MyGameObject obj = null;
    public MyAsset asset = null;
}