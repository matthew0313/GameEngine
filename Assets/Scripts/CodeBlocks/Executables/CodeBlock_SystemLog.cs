using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codeblock_SystemLog : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Debug;
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
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return message;
        yield return onFinish;
    }
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.integers["logType"] = logType.value;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        logType.value = save.data.integers["logType"];
    }
}