using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectorUIObject : InspectorUIElement, IObjectDraggable, IPointerDownHandler
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_Text objectName;

    ExposedObject element;
    MyGameObject obj;

    public void OnObjectDrag(MyGameObject obj)
    {
        if (element == null) return;
        if (element.typeSpecific && element.type != obj.type) return;
        element.setter(obj);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            EditorSceneManager.Instance.rightClickMenu.Open(Input.mousePosition, MakeRightClickMenu());
        }
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Clear",
            ctx =>
            {
                if (element == null) return;
                element.setter(null);
            });
    }

    public void Set(ExposedObject element)
    {
        label.text = element.name;
        if (element.typeSpecific) label.text += $" ({element.type})";
        this.element = element;
        obj = element.getter();
        objectName.text = obj != null ? obj.name : "None";
    }
    private void Update()
    {
        if (element == null) return;
        MyGameObject tmp = element.getter();
        if (tmp != obj)
        {
            obj = tmp;
            objectName.text = obj != null ? obj.name : "None";
        }
    }
}
