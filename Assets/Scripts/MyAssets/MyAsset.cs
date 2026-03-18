using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyAsset
{
    public readonly string filePath;
    public MyAsset(string filePath)
    {
        this.filePath = filePath;
    }
}
