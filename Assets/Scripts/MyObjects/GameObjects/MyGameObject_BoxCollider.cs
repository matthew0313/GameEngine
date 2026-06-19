using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MyGameObject_BoxCollider : MyGameObject
{
    [SerializeField] Transform boundingBox;
    public new BoxCollider2D collider { get; private set; }
    public override string id => "BoxCollider";
    protected override void Awake()
    {
        base.Awake();
        collider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        boundingBox.gameObject.SetActive((selected || EditorSceneManager.Instance.selected is MyGameObject_Rigidbody rb && rb.rb == collider.attachedRigidbody) && collider.enabled);
        boundingBox.localScale = collider.size;
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedBool(
            "enabled",
            () => collider.enabled,
            (value) => collider.enabled = value);
        yield return new ExposedBool(
            "IsTrigger",
            () => collider.isTrigger,
            (value) => collider.isTrigger = value);
    }
    bool selected = false;
    public override void OnSelect()
    {
        base.OnSelect();
        selected = true;
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        selected = false;
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.bools["enabled"] = collider.enabled;
        save.data.bools["isTrigger"] = collider.isTrigger;
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        if (save.data.bools.TryGetValue("enabled", out bool enabled)) collider.enabled = enabled;
        if (save.data.bools.TryGetValue("isTrigger", out bool isTrigger)) collider.isTrigger = isTrigger;
    }
}