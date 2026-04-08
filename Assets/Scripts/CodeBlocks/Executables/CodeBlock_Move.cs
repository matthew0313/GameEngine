using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_Move : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint moveX, moveY;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask Execute(ulong hash)
    {
        owner.transform.position += new Vector3(moveX.GetValue(), moveY.GetValue());
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}