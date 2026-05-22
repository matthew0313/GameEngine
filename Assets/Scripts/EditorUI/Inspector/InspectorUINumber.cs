using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUINumber : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_InputField input;
    ExposedNumber element;
    float value = 0.0f;
    public void Set(ExposedNumber element)
    {
        this.element = element;
        label.text = element.name;
        value = element.getter();
        input.text = value.ToString();
        input.onEndEdit.RemoveAllListeners();
        input.onEndEdit.AddListener(val =>
        {
            if (float.TryParse(val, out float result)) element.setter(result);
            else input.text = value.ToString();
        });
    }
    private void Update()
    {
        if (element == null) return;
        float tmp = element.getter();
        if (tmp != value)
        {
            value = tmp;
            input.text = value.ToString();
        }
    }
}
