using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_AddVelocityY : RigidbodyExecutableCodeBlock
{
    [SerializeField] NumericSnapPoint input;
    protected override void Apply(MyGameObject_Rigidbody rb, ulong hash)
    {
        var v = rb.rb.linearVelocity;
        v.y += input.GetNumber(hash);
        rb.rb.linearVelocity = v;
    }
    protected override IEnumerable<SnapPoint> Inputs() { yield return input; }
}
