using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectSnapPoint : SnapPoint, IObjectDraggable, IPointerDownHandler
{
    [SerializeField] TMP_Text setObjectText;
    MyGameObject setObject = null;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) &&
            codeBlock is PropertyCodeBlock propertyBlock &&
            (propertyBlock.propertyType & PropertyType.Object) > 0;
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
    void SetObject(MyGameObject obj)
    {
        if (setObject != null) setObject.onDelete -= OnSetObjectDelete;
        setObject = obj;
        setObjectText.text = setObject != null ? setObject.name : "None";
        if(setObject != null) setObject.onDelete += OnSetObjectDelete;
    }
    public void OnObjectDrag(MyGameObject obj)
    {
        if (snapped != null) return;
        SetObject(obj);
    }
    void OnSetObjectDelete() => SetObject(null);

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Middle && snapped == null)
        {
            eventData.Use();
            SetObject(null);
        }
    }
}