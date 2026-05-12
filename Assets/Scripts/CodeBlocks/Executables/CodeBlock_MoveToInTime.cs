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
        float counter = time.GetNumber(hash);
        if (counter <= 0)
        {
            owner.transform.position += new Vector3(targetX.GetNumber(hash), targetY.GetNumber(hash));
        }
        else
        {
            Vector3 move = (new Vector3(targetX.GetNumber(hash), targetY.GetNumber(hash)) - owner.transform.position) / counter;
            while (counter > 0)
            {
                await UniTask.Yield();
                owner.transform.position += move * Mathf.Max(counter, Time.deltaTime);
                counter -= Time.deltaTime;
            }
        }
        return await onFinish.Execute(hash);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}