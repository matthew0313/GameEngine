using System;
using UnityEngine;

public abstract class CodeBlock : MonoBehaviour
{
    public SnapPoint snappedPoint;
    public MyGameObject owner;
    public abstract string id { get; }
}