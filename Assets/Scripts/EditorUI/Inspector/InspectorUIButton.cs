using UnityEngine;
using UnityEngine.UI;

public class InspectorUIButton : InspectorUIElement
{
    [SerializeField] Button button;
    [SerializeField] Text label;

    public void Set(ExposedButton element)
    {
        if (label != null) label.text = element.name;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => element.onClick());
    }
}
