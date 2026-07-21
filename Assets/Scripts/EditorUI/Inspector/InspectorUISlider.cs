using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class InspectorUISlider : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text minValue, maxValue;
    [SerializeField] TMP_InputField input;
    ExposedSlider element;
    float value = 0.0f;
    public void Set(ExposedSlider element)
    {
        this.element = element;
        label.text = element.name;
        value = element.getter();
        slider.value = value;
        slider.minValue = element.min;
        minValue.text = element.min.ToString();
        slider.maxValue = element.max;
        maxValue.text = element.max.ToString();
        slider.wholeNumbers = element.snapToInt;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(val =>
        {
            element.setter(val);
        });
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
            slider.value = value;
            input.text = value.ToString();
        }
    }
}
