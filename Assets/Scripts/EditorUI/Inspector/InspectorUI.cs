using UnityEngine;

public class InspectorUI : MonoBehaviour
{
    private void OnEnable()
    {
        EditorSceneManager.Instance.onSelect += OnSelect;
        OnSelect(EditorSceneManager.Instance.selected);
    }
    private void OnDisable()
    {
        EditorSceneManager.Instance.onSelect -= OnSelect;
    }
    void OnSelect(ISelectable selected)
    {
        if (selected is IInspectable inspectable) Inspect(inspectable);
        else Clear();
    }
    void Inspect(IInspectable inspectable)
    {
        foreach (var element in inspectable.GetElements())
        {
            if (element is ExposedButton button)
            {

            }
            else if (element is ExposedVector2 vector2)
            {

            }
            else if (element is ExposedFloat exposedFloat)
            {

            }
            else if (element is ExposedBool exposedBool)
            {

            }
            else if (element is ExposedString exposedString)
            {

            }
            else if (element is ExposedObject exposedObject)
            {

            }
            else if (element is ExposedAnchor anchor)
            {

            }
        }
    }
    void Clear()
    {

    }
}
