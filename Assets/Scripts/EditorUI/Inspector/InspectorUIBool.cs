using UnityEngine;
using UnityEngine.UI;

public class InspectorUIBool : InspectorUIElement
{
    [SerializeField] Toggle toggle;

    public void Set(ExposedBool element)
    {
        toggle.isOn = element.getter();
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(val => element.setter(val));
    }
}
