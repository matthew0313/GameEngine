using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class MyGameObject_Rigidbody : MyGameObject
{
    public Rigidbody2D rb { get; private set; }
    public override string id => "Rigidbody";
    RigidbodyType2D setType = RigidbodyType2D.Dynamic;

    public event Action<Collision2D> onCollisionEnter, onCollisionStay, onCollisionExit;
    private void OnCollisionEnter2D(Collision2D collision) => onCollisionEnter?.Invoke(collision);
    private void OnCollisionStay2D(Collision2D collision) => onCollisionStay?.Invoke(collision);
    private void OnCollisionExit2D(Collision2D collision) => onCollisionExit?.Invoke(collision);

    public event Action<Collider2D> onTriggerEnter, onTriggerStay, onTriggerExit;
    private void OnTriggerEnter2D(Collider2D collision) => onTriggerEnter?.Invoke(collision);
    private void OnTriggerStay2D(Collider2D collision) => onTriggerStay?.Invoke(collision);
    private void OnTriggerExit2D(Collider2D collision) => onTriggerExit?.Invoke(collision);
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
                onInspectorChange?.Invoke();
            },
            new string[] { "Dynamic", "Kinematic", "Static" });
        bool isDynamic = setType == RigidbodyType2D.Dynamic;
        yield return new ExposedBool(
            "Use Auto Mass",
            () => rb.useAutoMass,
            (value) =>
            {
                rb.useAutoMass = value;
                onInspectorChange?.Invoke();
            }) 
        { visible = isDynamic };
        yield return new ExposedNumber(
            "Mass",
            () => rb.mass,
            (value) => rb.mass = value) 
        { visible = isDynamic && !rb.useAutoMass };
        yield return new ExposedNumber(
            "Linear Damping",
            () => rb.linearDamping,
            (value) => rb.linearDamping = value) 
        { visible = isDynamic };
        yield return new ExposedNumber(
            "Angular Damping",
            () => rb.angularDamping,
            (value) => rb.angularDamping = value) 
        { visible = isDynamic };
        yield return new ExposedNumber(
            "Gravity Scale",
            () => rb.gravityScale,
            (value) => rb.gravityScale = value) 
        { visible = isDynamic };
        yield return new ExposedBool(
            "Freeze Rotation",
            () => rb.freezeRotation,
            (value) => rb.freezeRotation = value)
        { visible = isDynamic };
        yield return new ExposedVector2(
            "Linear Velocity",
            () => rb.linearVelocity,
            (value) => rb.linearVelocity = value)
        { visible = isDynamic };
        yield return new ExposedNumber(
            "Angular Velocity",
            () => rb.angularVelocity,
            (value) => rb.angularVelocity = value)
        { visible = isDynamic };
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.integers["bodyType"] = (int)rb.bodyType;
        save.data.bools["useAutoMass"] = rb.useAutoMass;
        save.data.floats["mass"] = rb.mass;
        save.data.floats["linearDamping"] = rb.linearDamping;
        save.data.floats["angularDamping"] = rb.angularDamping;
        save.data.floats["gravityScale"] = rb.gravityScale;
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        rb.bodyType = (RigidbodyType2D)save.data.integers["bodyType"];
        rb.useAutoMass = save.data.bools["useAutoMass"];
        rb.mass = save.data.floats["mass"];
        rb.linearDamping = save.data.floats["linearDamping"];
        rb.angularDamping = save.data.floats["angularDamping"];
        rb.gravityScale = save.data.floats["gravityScale"];
    }
}