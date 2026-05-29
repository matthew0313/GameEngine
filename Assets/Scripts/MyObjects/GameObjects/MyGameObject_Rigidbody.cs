using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MyGameObject_Rigidbody : MyGameObject
{
    Rigidbody2D rb;
    public override MyGameObjectType type => MyGameObjectType.Rigidbody;
    RigidbodyType2D setType = RigidbodyType2D.Dynamic;

    public event Action<Collision2D> onCollisionEnter, onCollisionStay, onCollisionExit;
    private void OnCollisionEnter2D(Collision2D collision) => onCollisionEnter?.Invoke(collision);
    private void OnCollisionStay2D(Collision2D collision) => onCollisionStay?.Invoke(collision);
    private void OnCollisionExit2D(Collision2D collision) => onCollisionExit?.Invoke(collision);
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }
    public override void OnStart()
    {
        base.OnStart();
        rb.bodyType = setType;
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedDropdown(
            "Body type",
            () => (int)setType,
            (value) =>
            {
                setType = (RigidbodyType2D)value;
                if(EditorSceneManager.Instance.playMode) rb.bodyType = setType;
                OnInspectorChange();
            },
            new string[] { "Dynamic", "Kinematic", "Static" });
        if(setType == RigidbodyType2D.Dynamic)
        {
            yield return new ExposedBool(
                "Use Auto Mass",
                () => rb.useAutoMass,
                (value) =>
                {
                    rb.useAutoMass = value;
                    OnInspectorChange();
                });
            if (!rb.useAutoMass)
            {
                yield return new ExposedNumber(
                    "Mass",
                    () => rb.mass,
                    (value) => rb.mass = value);
            }
            yield return new ExposedNumber(
                "Linear Damping",
                () => rb.linearDamping,
                (value) => rb.linearDamping = value);
            yield return new ExposedNumber(
                "Angular Damping",
                () => rb.angularDamping,
                (value) => rb.angularDamping = value);
            yield return new ExposedNumber(
                "Gravity Scale",
                () => rb.gravityScale,
                (value) => rb.gravityScale = value);
        }
    }
}