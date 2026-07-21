using System;
using System.Collections.Generic;
using UnityEngine;

public class InspectorUI : MonoBehaviour
{
    [Header("UI Layout")]
    [SerializeField] Transform container;

    [Header("UI Prefabs")]
    [SerializeField] InspectorUIButton buttonPrefab;
    [SerializeField] InspectorUIVector2 vector2Prefab;
    [SerializeField] InspectorUINumber numberPrefab;
    [SerializeField] InspectorUIBool boolPrefab;
    [SerializeField] InspectorUIString stringPrefab;
    [SerializeField] InspectorUIObject objectPrefab;
    [SerializeField] InspectorUIAsset assetPrefab;
    [SerializeField] InspectorUIAnchor anchorPrefab;
    [SerializeField] InspectorUIDropdown dropdownPrefab;
    [SerializeField] InspectorUIColor colorPrefab;
    [SerializeField] InspectorUISlider sliderPrefab;
    [SerializeField] InspectorUIFoldout foldoutPrefab;

    Pooler<InspectorUIButton> buttonPool;
    Pooler<InspectorUIVector2> vector2Pool;
    Pooler<InspectorUINumber> numberPool;
    Pooler<InspectorUIBool> boolPool;
    Pooler<InspectorUIString> stringPool;
    Pooler<InspectorUIObject> objectPool;
    Pooler<InspectorUIAsset> assetPool;
    Pooler<InspectorUIAnchor> anchorPool;
    Pooler<InspectorUIDropdown> dropdownPool;
    Pooler<InspectorUIColor> colorPool;
    Pooler<InspectorUISlider> sliderPool;
    Pooler<InspectorUIFoldout> foldoutPool;

    readonly List<InspectorUIElement> active = new();

    Pooler<T> MakePool<T>(T prefab) where T : InspectorUIElement
    {
        var pool = new Pooler<T>(prefab);
        pool.onCreate = item => item.Init(this);
        pool.onTakeout = obj => { obj.gameObject.SetActive(true); obj.transform.SetAsLastSibling(); };
        return pool;
    }

    private void OnEnable()
    {
        buttonPool ??= MakePool(buttonPrefab);
        vector2Pool ??= MakePool(vector2Prefab);
        numberPool ??= MakePool(numberPrefab);
        boolPool ??= MakePool(boolPrefab);
        stringPool ??= MakePool(stringPrefab);
        objectPool ??= MakePool(objectPrefab);
        assetPool ??= MakePool(assetPrefab);
        anchorPool ??= MakePool(anchorPrefab);
        dropdownPool ??= MakePool(dropdownPrefab);
        colorPool ??= MakePool(colorPrefab);
        sliderPool ??= MakePool(sliderPrefab);
        foldoutPool ??= MakePool(foldoutPrefab);
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
    public IInspectable inspecting { get; private set; }
    public InspectorUIElement GetElement(ExposedElement element, Transform container)
    {
        if (element is ExposedButton exposedButton)
        {
            var ui = buttonPool.GetObject(container);
            ui.Set(exposedButton);
            return ui;
        }
        else if (element is ExposedVector2 exposedVector2)
        {
            var ui = vector2Pool.GetObject(container);
            ui.Set(exposedVector2);
            return ui;
        }
        else if (element is ExposedNumber exposedNumber)
        {
            var ui = numberPool.GetObject(container);
            ui.Set(exposedNumber);
            return ui;
        }
        else if (element is ExposedBool exposedBool)
        {
            var ui = boolPool.GetObject(container);
            ui.Set(exposedBool);
            return ui;
        }
        else if (element is ExposedString exposedString)
        {
            var ui = stringPool.GetObject(container);
            ui.Set(exposedString);
            return ui;
        }
        else if (element is ExposedObject exposedObject)
        {
            var ui = objectPool.GetObject(container);
            ui.Set(exposedObject);
            return ui;
        }
        else if (element is ExposedAsset exposedAsset)
        {
            var ui = assetPool.GetObject(container);
            ui.Set(exposedAsset);
            return ui;
        }
        else if (element is ExposedAnchor exposedAnchor)
        {
            var ui = anchorPool.GetObject(container);
            ui.Set(exposedAnchor);
            return ui;
        }
        else if (element is ExposedDropdown exposedDropdown)
        {
            var ui = dropdownPool.GetObject(container);
            ui.Set(exposedDropdown);
            return ui;
        }
        else if (element is ExposedColor exposedColor)
        {
            var ui = colorPool.GetObject(container);
            ui.Set(exposedColor);
            return ui;
        }
        else if (element is ExposedSlider exposedSlider)
        {
            var ui = sliderPool.GetObject(container);
            ui.Set(exposedSlider);
            return ui;
        }
        else if(element is ExposedFoldout exposedFoldout)
        {
            var ui = foldoutPool.GetObject(container);
            ui.Set(exposedFoldout);
            return ui;
        }
        return null;
    }
    public void ReleaseElement(InspectorUIElement ui)
    {
        if (ui is InspectorUIButton btn) buttonPool.ReleaseObject(btn);
        else if (ui is InspectorUIVector2 v2) vector2Pool.ReleaseObject(v2);
        else if (ui is InspectorUINumber n) numberPool.ReleaseObject(n);
        else if (ui is InspectorUIBool b) boolPool.ReleaseObject(b);
        else if (ui is InspectorUIString s) stringPool.ReleaseObject(s);
        else if (ui is InspectorUIObject o) objectPool.ReleaseObject(o);
        else if (ui is InspectorUIAsset a) assetPool.ReleaseObject(a);
        else if (ui is InspectorUIAnchor anchor) anchorPool.ReleaseObject(anchor);
        else if (ui is InspectorUIDropdown dropdown) dropdownPool.ReleaseObject(dropdown);
        else if (ui is InspectorUIColor color) colorPool.ReleaseObject(color);
        else if (ui is InspectorUISlider slider) sliderPool.ReleaseObject(slider);
        else if (ui is InspectorUIFoldout foldout) foldoutPool.ReleaseObject(foldout);
    }
    void Inspect(IInspectable inspectable)
    {
        Clear();
        if (inspectable == null) return;
        inspecting = inspectable;
        inspecting.onInspectorChange += Reload;
        foreach (var element in inspectable.GetElements())
        {
            if (!element.visible) continue;
            var ui = GetElement(element, container);
            active.Add(ui);
        }
    }
    void Reload()
    {
        if (inspecting != null) Inspect(inspecting);
    }
    void Clear()
    {
        foreach (var ui in active) ReleaseElement(ui);
        active.Clear();
        if (inspecting != null) inspecting.onInspectorChange -= Reload;
        inspecting = null;
    }
}
