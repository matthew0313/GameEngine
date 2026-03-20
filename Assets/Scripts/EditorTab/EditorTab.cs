using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorTab : MonoBehaviour
{
    public virtual void Open() { }
    public virtual void Close() { }
    public virtual void Focus() { }
    public virtual void FocusUpdate() { }
    public virtual void UnFocus() { }
}