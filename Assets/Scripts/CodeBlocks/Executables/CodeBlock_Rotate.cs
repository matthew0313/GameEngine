using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_Rotate : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Movement;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint angle;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    public override bool IsAddable(ICodeable codeable) => codeable is MyGameObject;
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash, CancellationToken token)
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
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return angle;
        yield return onFinish;
    }
}