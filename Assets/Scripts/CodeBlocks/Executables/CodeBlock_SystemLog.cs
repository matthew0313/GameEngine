using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_SystemLog : ExecutableCodeBlock, IOnFinish
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Dropdown logType;
    [SerializeField] StringSnapPoint message;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        EditorSceneManager.Instance.AddLog(new MyLog((MyLogType)logType.value, message.GetValue(hash)));
        return await onFinish.Execute(hash);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}