using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIBool : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] Toggle toggle;
    ExposedBool element;
    public void Set(ExposedBool element)
    {
        this.element = element;
        label.text = element.name;
        toggle.isOn = element.getter();
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(val => element.setter(val));
    }
    private void Update()
    {
        if (element == null) return;
        toggle.isOn = element.getter();
    }
}
