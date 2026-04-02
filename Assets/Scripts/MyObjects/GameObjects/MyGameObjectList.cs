using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyGameObjectList", menuName = "Scriptables/MyGameObjectList")]
public class MyGameObjectList : ScriptableObject
{
    public List<MyGameObject> myGameObjects;
}