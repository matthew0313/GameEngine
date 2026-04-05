using System;
using System.Collections.Generic;
using UnityEngine;

public class MyScene : MonoBehaviour
{
    [SerializeField] Transform objectAnchor, UIAnchor;
    public List<MyGameObject> topGameObjects = new();
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
            AddTopObject(obj);
            saves.Add(obj, i);
        }
        foreach (var i in saves) i.Key.Load(i.Value);
    }
    public void AddTopObject(MyGameObject gameObject)
    {
        if (gameObject is MyGameObject_UI) gameObject.transform.SetParent(UIAnchor, true);
        else gameObject.transform.SetParent(objectAnchor, true);
        topGameObjects.Add(gameObject);
        gameObject.onChildrenChange += OnChildrenChange;
        onHierarchyChange?.Invoke();
    }

    public void RemoveTopObject(MyGameObject gameObject)
    {
        if (!topGameObjects.Contains(gameObject)) return;
        topGameObjects.Remove(gameObject);
        gameObject.onChildrenChange -= OnChildrenChange;
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
