using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIDropdown : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_Dropdown dropdown;
    ExposedDropdown element;
    public void Set(ExposedDropdown element)
    {
        this.element = element;
        label.text = element.name;
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.ClearOptions();
        dropdown.AddOptions(element.options.ToList());
        dropdown.value = element.getter();
        dropdown.onValueChanged.AddListener(value =>
        {
            element.setter(value);
        });
    }
    private void Update()
    {
        if (element == null) return;
        dropdown.value = element.getter();
    }
}
