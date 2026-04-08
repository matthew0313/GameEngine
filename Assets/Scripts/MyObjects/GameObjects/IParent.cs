using System;
using System.Collections.Generic;
using UnityEngine;

public interface IParent
{
    public void AddChild(MyGameObject child);
    public void RemoveChild(MyGameObject child);
    public bool HasChild(MyGameObject child);
    public int GetChildIndex(MyGameObject child);
    public void SetChildIndex(MyGameObject child, int index);
}