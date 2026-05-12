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
            if(time.GetValue(hash)<0){
                go.transform.position += new Vector3(moveX.GetValue(hash), moveY.GetValue(hash));
            }
            else{
                float elapsed =0;
                Vector3 targetPosition = go.transform.position + new Vector3(moveX.GetValue(hash), moveY.GetValue(hash));
                while(elapsed<time.GetValue(hash)){
                    elapsed+=Time.deltaTime;
                    go.transform.position += new Vector3(go.transform.position,targetPosition,elapsed/time.GetValue(hash));
                    await UniTask.Yield();
                }
            }
            go.transform.position += new Vector3(moveX.GetValue(hash), moveY.GetValue(hash));
        }
        return await onFinish.Execute(hash);
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}