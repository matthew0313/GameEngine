using TMPro;
using UnityEngine;

public class Codeblock_Move : ExecutableCodeBlock
{
    public override string id => "Move";

    [SerializeField] RectTransform rectTransform;
    [SerializeField] NumericSnapPoint moveX, moveY;
    [SerializeField] ExecutableSnapPoint onFinish;
    public override void Execute()
    {
        owner.transform.position += new Vector3(moveX.GetValue(), moveY.GetValue());
    }
    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
}