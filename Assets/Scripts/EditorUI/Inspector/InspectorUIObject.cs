using UnityEngine;

public class InspectorUIObject : InspectorUIElement
{
    ExposedObject element;
    public void Set(ExposedObject element)
    {
        this.element = element;
    }
}
