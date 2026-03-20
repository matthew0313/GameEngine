using System;
using UnityEngine;

public abstract class CodeBlock : MonoBehaviour
{
    [HideInInspector] public SnapPoint snappedPoint;
    [HideInInspector] public MyGameObject owner;
    public abstract string id { get; }
    public virtual CodeBlockSave Save()
    {
        CodeBlockSave save = new();
        save.id = id;
        return save;
    }
    public virtual void Load(CodeBlockSave save) { }
}
[System.Serializable]
public class CodeBlockSave
{
    public string id;
    public Vector2 position;
    public DataUnit data = new();   
}