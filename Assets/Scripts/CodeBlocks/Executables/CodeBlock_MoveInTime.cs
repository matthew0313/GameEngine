using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class Codeblock_MoveInTime : ExecutableCodeBlock, IOnFinish
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint time;
    [SerializeField] NumericSnapPoint moveX, moveY;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }

    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        if(owner is MyGameObject go) {
            if(time.GetNumber(hash)<0){
                go.transform.position += new Vector3(moveX.GetNumber(hash), moveY.GetNumber(hash));
            }
            else{
                float elapsed =0;
                Vector3 targetPosition = go.transform.position + new Vector3(moveX.GetNumber(hash), moveY.GetNumber(hash));
                while(elapsed<time.GetNumber(hash)){
                    elapsed+=Time.deltaTime;
                    go.transform.position += Vector3.Lerp(go.transform.position,targetPosition,elapsed/time.GetNumber(hash));
                    await UniTask.Yield();
                }
            }
            go.transform.position += new Vector3(moveX.GetNumber(hash), moveY.GetNumber(hash));
        }
        return await onFinish.Execute(hash);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}