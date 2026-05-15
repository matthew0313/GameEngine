using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InspectorUI : MonoBehaviour
{
    [Header("UI Layout")]
    [SerializeField] private Transform container;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject vector2Prefab;
    [SerializeField] private GameObject floatPrefab;
    [SerializeField] private GameObject boolPrefab;
    [SerializeField] private GameObject stringPrefab;
    [SerializeField] private GameObject anchorPrefab;
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
            if (element is ExposedButton exposedButton)
            {
                GameObject go = Instantiate(buttonPrefab, container);
                Button btn = go.GetComponentInChildren<Button>();
                Text label = go.GetComponentInChildren<Text>();
                
                if (label != null) label.text = exposedButton.Name;
                btn.onClick.AddListener(() => exposedButton.Invoke());
            }
            else if (element is ExposedVector2 exposedVector2)
            {
                GameObject go = Instantiate(vector2Prefab, container);
                InputField[] inputs = go.GetComponentsInChildren<InputField>();
                inputs[0].text = exposedVector2.getter().x.ToString();
                inputs[1].text = exposedVector2.getter().y.ToString();
                
                inputs[0].onEndEdit.AddListener(val => {
                    if(float.TryParse(val, out float x)) exposedVector2.setter(new Vector2(x, exposedVector2.getter().y));
                    else inputs[0].text = exposedVector2.getter().x.ToString();
                });
                inputs[1].onEndEdit.AddListener(val => {
                    fif(float.TryParse(val, out float y) exposedVector2.setter(new Vector2(exposedVector2.getter().x, y));
                    else inputs[1].text = exposedVector2.getter().y.ToString();
                });
            }
            else if (element is ExposedFloat exposedFloat)
            {
                GameObject go = Instantiate(floatPrefab, container);
                InputField input = go.GetComponentInChildren<InputField>();
                input.text = exposedFloat.getter().ToString();
                input.onEndEdit.AddListener(val => {
                    exposedFloat.setter(float.TryParse(val, out float result) ? result : 0);
                })
            }
            else if (element is ExposedBool exposedBool)
            {
                GameObject go = Instantiate(boolPrefab, container);
                Toggle toggle = go.GetComponentInChildren<Toggle>();
                toggle.isOn = exposedBool.Value;
                toggle.onValueChanged.AddListener(val => exposedBool.Value = val);
            }
            else if (element is ExposedString exposedString)
            {
                GameObject go = Instantiate(stringPrefab, container);
                InputField input = go.GetComponentInChildren<InputField>();
                input.text = exposedString.Value;
                input.onEndEdit.AddListener(val => exposedString.Value = val);
            }
            else if (element is ExposedObject exposedObject)
            {
                //
            }
            else if(element is ExposedAsset exposedAsset)
            {
                //
            }
            else if (element is ExposedAnchor exposedAnchor)
            {
                
            }
        }
    }
    void Clear()
    {
        if (container == null) return;
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
