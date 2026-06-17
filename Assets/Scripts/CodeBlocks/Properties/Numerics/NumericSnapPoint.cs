using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumericSnapPoint : SnapPoint
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    [SerializeField] TMP_InputField inputField;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && 
            codeBlock is PropertyCodeBlock propertyBlock && 
            (propertyBlock.propertyType & PropertyType.Number) > 0;
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
    public float GetNumber(ulong hash)
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetNumber(hash);
        }
        if (float.TryParse(inputField.text, out float value)) return value;
        return 0.0f;
    }
    public int GetIntNumber(ulong hash) => Mathf.FloorToInt(GetNumber(hash));
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
        inputField.text = string.Empty;
    }
    public override SnapPointSave Save()
    {
        var save = base.Save();
        save.data.floats["value"] = float.TryParse(inputField.text, out float value) ? value : 0.0f;
        return save;
    }
    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        inputField.text = save.data.floats["value"].ToString();
    }
}