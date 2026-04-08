using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_Move : ExecutableCodeBlock
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint moveX, moveY;
    [SerializeField] ExecutableSnapPoint onFinish;
    public override async UniTask Execute(ulong hash)
    {
        owner.transform.position += new Vector3(moveX.GetValue(), moveY.GetValue());
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}