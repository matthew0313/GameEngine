using TMPro;
using UnityEngine;

public class InspectorUIObject : InspectorUIElement, IObjectDraggable
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_Text objectName;

    ExposedObject element;
    MyGameObject obj;

    public void OnObjectDrag(MyGameObject obj)
    {
        if (element == null) return;
        element.setter(obj);
    }
    public void Set(ExposedObject element)
    {
        label.text = element.name;
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
