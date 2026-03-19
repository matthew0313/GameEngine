using System;
using UnityEngine;

public abstract class CodeBlock : MonoBehaviour
{
    [HideInInspector] public SnapPoint snappedPoint;
    [HideInInspector] public MyGameObject owner;
    public abstract string id { get; }
}