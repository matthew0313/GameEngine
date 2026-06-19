using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Codeblock_Wait : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Logic;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Dropdown unit;
    [SerializeField] NumericSnapPoint duration;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
    {
        float waitTime = duration.GetNumber(hash);
        switch (unit.value)
        {
            case 0:
                await UniTask.Delay(TimeSpan.FromMilliseconds(waitTime), cancellationToken: token);
                break;
            case 1:
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
                break;
            case 2:
                await UniTask.Delay(TimeSpan.FromMinutes(waitTime), cancellationToken: token);
                break;
        }
        return await onFinish.Execute(hash, token);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return duration;
        yield return onFinish;
    }
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.integers["unit"] = unit.value;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        if (save.data.integers.TryGetValue("unit", out int unitValue)) unit.value = unitValue;
    }
}