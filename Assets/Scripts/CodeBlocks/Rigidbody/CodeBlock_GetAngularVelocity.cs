public class CodeBlock_GetAngularVelocity : RigidbodyNumericCodeBlock
{
    protected override float GetValue(MyGameObject_Rigidbody rb) => rb.rb.angularVelocity;
}
