using System;
using System.Collections.Generic;
using UnityEngine;

public class MyScene : MonoBehaviour, IParent
{
    [SerializeField] Transform objectAnchor, UIAnchor;
    public readonly List<MyGameObject> topGameObjects = new();
    public event Action onHierarchyChange;
    public MySceneSave Save(bool prettyPrint = false)
    {
        MySceneSave save = new();
        foreach(var i in topGameObjects) save.topGameObjects.Add(i.Save(prettyPrint));
        return save;
    }
    public void Load(MySceneSave save)
    {
        Dictionary<MyGameObject, MyGameObjectSave> saves = new();
        foreach(var i in save.topGameObjects)
        {
            MyGameObject obj = Instantiate(EditorSceneManager.Instance.IDToGameObject(i.id));
            obj.EarlyLoad(i);
            AddChild(obj);
            saves.Add(obj, i);
        }
        foreach (var i in saves) i.Key.Load(i.Value);
    }
    public void AddChild(MyGameObject child)
    {
        if (topGameObjects.Contains(child)) return;
        if (child.parent != null && child.parent != this) child.parent.RemoveChild(child);
        if (child is MyGameObject_UI) child.transform.SetParent(UIAnchor, true);
        else child.transform.SetParent(objectAnchor, true);
        child.parent = this;
        topGameObjects.Add(child);
        child.onChildrenChange += OnChildrenChange;
        onHierarchyChange?.Invoke();
    }

    public void RemoveChild(MyGameObject child)
    {
        if (!topGameObjects.Contains(child)) return;
        child.transform.SetParent(null, true);
        child.parent = null;
        topGameObjects.Remove(child);
        child.onChildrenChange -= OnChildrenChange;
        onHierarchyChange?.Invoke();
    }
    public bool HasChild(MyGameObject child) => topGameObjects.Contains(child);
    public int GetChildIndex(MyGameObject obj) => topGameObjects.IndexOf(obj);
    public void SetChildIndex(MyGameObject child, int index)
    {
        if (!topGameObjects.Contains(child) || index < 0 || index >= topGameObjects.Count) return;
        topGameObjects.Remove(child); topGameObjects.Insert(index, child);
        child.transform.SetSiblingIndex(index);
        onHierarchyChange?.Invoke();
    }
    private void OnChildrenChange() => onHierarchyChange?.Invoke();
    public bool ContainsObject(MyGameObject obj) => FindObject((o) => o == obj) != null;
    public MyGameObject FindObject(Func<MyGameObject, bool> predicate)
    {
        foreach(var obj in GetObjects())
        {
            if (predicate.Invoke(obj)) return obj;
        }
        return null;
    }
    public IEnumerable<MyGameObject> GetObjects()
    {
        foreach(var obj in topGameObjects)
        {
            foreach (var i in obj.GetHierarchy()) yield return i;
        }
    }
    public MyGameObject UIDToObject(ulong uid)
    {
        foreach(var obj in GetObjects())
        {
            if (obj.uid == uid) return obj;
        }
        return null;
    }
}
[System.Serializable]
public class MySceneSave
{
    public List<MyGameObjectSave> topGameObjects = new();
}
