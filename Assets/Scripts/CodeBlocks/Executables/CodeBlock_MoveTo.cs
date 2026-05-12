using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_MoveTo : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint targetX, targetY;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }

    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        MyGameObject owner = this.owner as MyGameObject;
        if (owner == null)
        {
            EditorSceneManager.Instance.AddLog(new MyLog(MyLogType.Error, "Movement block executed in non-object."));
            return new() { exception = true };
        }
        owner.transform.position = new Vector3(targetX.GetNumber(hash), targetY.GetNumber(hash));
        return await onFinish.Execute(hash);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}