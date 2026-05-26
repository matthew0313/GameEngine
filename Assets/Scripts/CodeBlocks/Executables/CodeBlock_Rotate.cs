using UnityEngine;

public class CodeBlock_Rotate : ExecutableCodeBlock
{
    [SerializeField] NumericSnapPoint angle;

    public override Unitask<ExecutionFinishedInfo> Execute(ulong hash)
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
}