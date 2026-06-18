public class CodeBlock_GetVelocityY : RigidbodyNumericCodeBlock
{
    protected override float GetValue(MyGameObject_Rigidbody rb) => rb.rb.linearVelocity.y;
}
