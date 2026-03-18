using System;
using System.Collections.Generic;
using UnityEngine;

public class MyScene
{
    public List<MyGameObject> myGameObjects = new();
    public MySceneSave Save()
    {
        MySceneSave save = new();
        foreach(var i in myGameObjects) save.gameObjects.Add(i.Save());
        return save;
    }
    public void Load(MySceneSave save)
    {
        foreach(var i in save.gameObjects)
        {
            MyGameObject obj = new GameObject("MyGameObject").AddComponent<MyGameObject>();
            obj.Load(i);
            myGameObjects.Add(obj);
        }
    }
}
[System.Serializable]
public class MySceneSave
{
    public List<MyGameObjectSave> gameObjects = new();
}
