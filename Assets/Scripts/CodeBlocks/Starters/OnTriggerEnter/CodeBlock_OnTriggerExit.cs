public class Codeblock_OnTriggerExit : CodeBlock_OnTrigger
{
    protected override void Subscribe(MyGameObject_Rigidbody rb) => rb.onTriggerExit += OnObjectTrigger;
    protected override void Unsubscribe(MyGameObject_Rigidbody rb) => rb.onTriggerExit -= OnObjectTrigger;
}
