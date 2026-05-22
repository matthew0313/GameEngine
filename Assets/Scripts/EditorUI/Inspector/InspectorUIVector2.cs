using UnityEngine;
using TMPro;

public class InspectorUIVector2 : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_InputField inputX;
    [SerializeField] TMP_InputField inputY;
    ExposedVector2 element;
    Vector2 value;
    public void Set(ExposedVector2 element)
    {
        this.element = element;
        label.text = element.name;
        value = element.getter();
        inputX.text = value.x.ToString();
        inputY.text = value.y.ToString();

        inputX.onEndEdit.RemoveAllListeners();
        inputX.onEndEdit.AddListener(val =>
        {
            if (float.TryParse(val, out float x)) element.setter(new Vector2(x, element.getter().y));
            else inputX.text = value.x.ToString();
        });

        inputY.onEndEdit.RemoveAllListeners();
        inputY.onEndEdit.AddListener(val =>
        {
            if (float.TryParse(val, out float y)) element.setter(new Vector2(element.getter().x, y));
            else inputY.text = value.y.ToString();
        });
    }
    private void Update()
    {
        if (element == null) return;
        Vector2 tmp = element.getter();
        if(tmp != value)
        {
            value = tmp;
            inputX.text = value.x.ToString();
            inputY.text = value.y.ToString();
        }
    }
}
