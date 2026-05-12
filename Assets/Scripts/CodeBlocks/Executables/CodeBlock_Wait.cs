using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_Wait : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Dropdown unit;
    [SerializeField] NumericSnapPoint duration;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        float waitTime = duration.GetValue(hash);
        switch (unit.value)
        {
            case 0:
                await UniTask.Delay(TimeSpan.FromMilliseconds(waitTime));
                break;
            case 1:
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
                break;
            case 2:
                await UniTask.Delay(TimeSpan.FromMinutes(waitTime));
                break;
        }
        return await onFinish.Execute(hash);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}