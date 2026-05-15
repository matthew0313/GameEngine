using UnityEngine;
using UnityEngine.UI;

public class InspectorUIString : InspectorUIElement
{
    [SerializeField] InputField input;

    public void Set(ExposedString element)
    {
        input.text = element.getter();
        input.onEndEdit.RemoveAllListeners();
        input.onEndEdit.AddListener(val => element.setter(val));
    }
}
