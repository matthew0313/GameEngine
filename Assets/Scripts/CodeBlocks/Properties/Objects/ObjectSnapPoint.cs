using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectSnapPoint : SnapPoint, IObjectDraggable, IPointerDownHandler
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    [SerializeField] TMP_Text setObjectText;
    MyGameObject setObject = null;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) &&
            codeBlock is PropertyCodeBlock propertyBlock &&
            (propertyBlock.propertyType & PropertyType.Object) > 0;
    }
    private void Update()
    {
        if (layoutElement != null) layoutElement.minWidth = GetWidth();
    }
    protected override void OnSnappedChange()
    {
        unSnapped.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    public MyGameObject GetObject(ulong hash)
    {
        if(snapped == null)
        {
            return setObject;
        }
        else if(snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetObject(hash);
        }
        return null;
    }
    public void SetObject(MyGameObject obj)
    {
        if (setObject != null)
        {
            setObject.onDelete -= OnSetObjectDelete;
            setObject.onDisplayChange -= OnSetObjectDisplayChange;
        }
        setObject = obj;
        OnSetObjectDisplayChange();
        if(setObject != null)
        {
            setObject.onDelete += OnSetObjectDelete;
            setObject.onDisplayChange += OnSetObjectDisplayChange;
        }
    }
    public void OnObjectDrag(MyGameObject obj)
    {
        if (snapped != null) return;
        SetObject(obj);
    }
    void OnSetObjectDelete() => SetObject(null);
    void OnSetObjectDisplayChange() => setObjectText.text = setObject != null ? setObject.name : "None";
    public float GetWidth()
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetWidth();
        }
        else return defaultWidth;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Middle && snapped == null)
        {
            eventData.Use();
            SetObject(null);
        }
    }
    public override void Clear()
    {
        base.Clear();
        SetObject(null);
    }
    public override SnapPointSave Save()
    {
        var save = base.Save();
        save.data.ulongs["setObject"] = setObject != null ? setObject.uid : 0;
        return save;
    }
    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        ulong objID = save.data.ulongs["setObject"];
        SetObject(EditorSceneManager.Instance.FindObjectWithUID(objID));
    }
}