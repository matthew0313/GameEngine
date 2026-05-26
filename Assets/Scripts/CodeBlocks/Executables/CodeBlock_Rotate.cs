using Cysharp.Threading.Tasks;
using UnityEngine;

public class CodeBlock_Rotate : ExecutableCodeBlock, IOnFinish
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint angle;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        MyGameObject owner = this.owner as MyGameObject;
        if(owner == null)
        {
            EditorSceneManager.Instance.AddLog(new MyLog(MyLogType.Error, "Rotation block executed in non-object."));
            return new() { exception = true };
        }
        owner.transform.Rotate(Vector3.forward, angle.GetNumber(hash));
        return new();
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}