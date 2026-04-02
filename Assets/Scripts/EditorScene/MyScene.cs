using System;
using System.Collections.Generic;
using UnityEngine;

public class MyScene : MonoBehaviour
{
    [SerializeField] Transform objectAnchor, UIAnchor;
    public List<MyGameObject> topGameObjects = new();
    public MySceneSave Save(bool prettyPrint = false)
    {
        MySceneSave save = new();
        foreach(var i in topGameObjects) save.topGameObjects.Add(i.Save(prettyPrint));
        return save;
    }
    public void Load(MySceneSave save)
    {
        foreach(var i in save.topGameObjects)
        {
            MyGameObject obj = Instantiate(EditorSceneManager.Instance.IDToGameObject(i.id));
            obj.Load(i);
            AddTopObject(obj);
        }
    }
    public void AddTopObject(MyGameObject gameObject)
    {
        if (gameObject is MyGameObject_UI) gameObject.transform.SetParent(UIAnchor, true);
        else gameObject.transform.SetParent(objectAnchor, true);
        topGameObjects.Add(gameObject);
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
