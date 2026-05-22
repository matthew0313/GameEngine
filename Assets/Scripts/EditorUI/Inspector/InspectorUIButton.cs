using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIButton : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] Button button;

    public void Set(ExposedButton element)
    {
        label.text = element.name;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => element.onClick());
    }
}
