public class CodeBlock_GetVelocityX : RigidbodyNumericCodeBlock
{
    protected override float GetValue(MyGameObject_Rigidbody rb) => rb.rb.linearVelocity.x;
}
