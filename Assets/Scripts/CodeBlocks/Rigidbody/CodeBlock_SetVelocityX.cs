using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_SetVelocityX : RigidbodyExecutableCodeBlock
{
    [SerializeField] NumericSnapPoint input;
    protected override void Apply(MyGameObject_Rigidbody rb, ulong hash)
    {
        var v = rb.rb.linearVelocity;
        v.x = input.GetNumber(hash);
        rb.rb.linearVelocity = v;
    }
    protected override IEnumerable<SnapPoint> Inputs() { yield return input; }
}
