using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIString : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_InputField input;

    ExposedString element;
    string tmp;
    public void Set(ExposedString element)
    {
        this.element = element;
        label.text = element.name;
        tmp = element.getter();
        input.text = tmp;
        input.onEndEdit.RemoveAllListeners();
        input.onEndEdit.AddListener(val => element.setter(val));
    }
    private void Update()
    {
        if (element == null) return;
        string value = element.getter();
        if(value != tmp)
        {
            tmp = value;
            input.text = tmp;
        }
    }
}
