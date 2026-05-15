using UnityEngine;
using UnityEngine.UI;

public class InspectorUIFloat : InspectorUIElement
{
    [SerializeField] InputField input;

    public void Set(ExposedFloat element)
    {
        input.text = element.getter().ToString();
        input.onEndEdit.RemoveAllListeners();
        input.onEndEdit.AddListener(val =>
        {
            if (float.TryParse(val, out float result)) element.setter(result);
            else input.text = element.getter().ToString();
        });
    }
}
