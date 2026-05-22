using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIString : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_InputField input;

    ExposedString element;
    public void Set(ExposedString element)
    {
        this.element = element;
        label.text = element.name;
        input.text = element.getter();
        input.onEndEdit.RemoveAllListeners();
        input.onEndEdit.AddListener(val => element.setter(val));
    }
    private void Update()
    {
        if (element == null) return;
        input.text = element.getter();
    }
}
