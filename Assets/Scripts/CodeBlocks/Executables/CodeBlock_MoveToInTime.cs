using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_MoveToInTime : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint time;
    [SerializeField] NumericSnapPoint targetX, targetY;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }

    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        MyGameObject owner = this.owner as MyGameObject;
        if(owner == null)
        {
            EditorSceneManager.Instance.AddLog(new MyLog(MyLogType.Error, "Movement block executed in non-object."));
            return new() { exception = true };
        }
        float time2 = time.GetNumber(hash);
        if (time2 <= 0)
        {
            owner.transform.position = new Vector3(targetX.GetNumber(hash), targetY.GetNumber(hash));
        }
        else
        {
            float counter = 0.0f;
            Vector3 startPos = owner.transform.position;
            Vector3 targetPos = new Vector3(targetX.GetNumber(hash), targetY.GetNumber(hash));
            while (counter < time2)
            {
                await UniTask.Yield();
                owner.transform.position = Vector3.Lerp(startPos, targetPos, counter / time2);
                counter += Time.deltaTime;
            }
            owner.transform.position = targetPos;
        }
        return await onFinish.Execute(hash);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}