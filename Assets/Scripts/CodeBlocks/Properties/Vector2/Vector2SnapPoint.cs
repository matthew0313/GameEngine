using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Vector2SnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    [SerializeField] TMP_InputField inputX, inputY;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && 
            codeBlock is PropertyCodeBlock propertyBlock && 
            (propertyBlock.propertyType & PropertyType.Vector2) > 0;
    }
    protected override void OnSnappedChange()
    {
        unSnapped.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        if(layoutElement != null) layoutElement.minWidth = GetWidth();
    }
    public Vector2 GetVector2(ulong hash)
    {
        if(snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetVector2(hash);
        }
        Vector2 tmp = new();
        float.TryParse(inputX.text, out tmp.x);
        float.TryParse(inputY.text, out tmp.y);
        return tmp;
    }
    public float GetWidth()
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetWidth();
        }
        else return defaultWidth;
    }
    public override void Clear()
    {
        base.Clear();
        inputX.text = string.Empty;
        inputY.text = string.Empty;
    }
    public override SnapPointSave Save()
    {
        var save = base.Save();
        save.data.floats["inputX"] = float.TryParse(inputX.text, out float x) ? x : 0.0f;
        save.data.floats["inputY"] = float.TryParse(inputY.text, out float y) ? y : 0.0f;
        return save;
    }
    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        if (save.data.floats.TryGetValue("inputX", out float x)) inputX.text = x.ToString();
        if (save.data.floats.TryGetValue("inputY", out float y)) inputY.text = y.ToString();
    }
}