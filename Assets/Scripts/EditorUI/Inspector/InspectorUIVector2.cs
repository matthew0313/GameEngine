using UnityEngine;
using UnityEngine.UI;

public class InspectorUIVector2 : InspectorUIElement
{
    [SerializeField] InputField inputX;
    [SerializeField] InputField inputY;

    public void Set(ExposedVector2 element)
    {
        inputX.text = element.getter().x.ToString();
        inputY.text = element.getter().y.ToString();

        inputX.onEndEdit.RemoveAllListeners();
        inputX.onEndEdit.AddListener(val =>
        {
            if (float.TryParse(val, out float x)) element.setter(new Vector2(x, element.getter().y));
            else inputX.text = element.getter().x.ToString();
        });

        inputY.onEndEdit.RemoveAllListeners();
        inputY.onEndEdit.AddListener(val =>
        {
            if (float.TryParse(val, out float y)) element.setter(new Vector2(element.getter().x, y));
            else inputY.text = element.getter().y.ToString();
        });
    }
}
