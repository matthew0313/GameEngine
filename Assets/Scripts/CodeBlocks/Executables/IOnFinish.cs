using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IOnFinish
{
    public ExecutableSnapPoint onFinish { get; }
}