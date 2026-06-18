using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_AddAngularVelocity : RigidbodyExecutableCodeBlock
{
    [SerializeField] NumericSnapPoint input;
    protected override void Apply(MyGameObject_Rigidbody rb, ulong hash)
    {
        rb.rb.angularVelocity += input.GetNumber(hash);
    }
    protected override IEnumerable<SnapPoint> Inputs() { yield return input; }
}
