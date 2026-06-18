public class Codeblock_OnTriggerEnter : CodeBlock_OnTrigger
{
    protected override void Subscribe(MyGameObject_Rigidbody rb) => rb.onTriggerEnter += OnObjectTrigger;
    protected override void Unsubscribe(MyGameObject_Rigidbody rb) => rb.onTriggerEnter -= OnObjectTrigger;
}
