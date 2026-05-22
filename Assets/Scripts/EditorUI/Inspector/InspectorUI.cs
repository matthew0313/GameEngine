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

    Pooler<InspectorUIButton> buttonPool;
    Pooler<InspectorUIVector2> vector2Pool;
    Pooler<InspectorUINumber> numberPool;
    Pooler<InspectorUIBool> boolPool;
    Pooler<InspectorUIString> stringPool;
    Pooler<InspectorUIObject> objectPool;
    Pooler<InspectorUIAsset> assetPool;
    Pooler<InspectorUIAnchor> anchorPool;

    readonly List<InspectorUIElement> active = new();

    Pooler<T> MakePool<T>(T prefab) where T : InspectorUIElement
    {
        var pool = new Pooler<T>(prefab);
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
    IInspectable inspecting;
    void Inspect(IInspectable inspectable)
    {
        Clear();
        if (inspectable == null) return;
        foreach (var element in inspectable.GetElements())
        {
            if (element is ExposedButton exposedButton)
            {
                var ui = buttonPool.GetObject(container);
                ui.Set(exposedButton);
                active.Add(ui);
            }
            else if (element is ExposedVector2 exposedVector2)
            {
                var ui = vector2Pool.GetObject(container);
                ui.Set(exposedVector2);
                active.Add(ui);
            }
            else if (element is ExposedNumber exposedNumber)
            {
                var ui = numberPool.GetObject(container);
                ui.Set(exposedNumber);
                active.Add(ui);
            }
            else if (element is ExposedBool exposedBool)
            {
                var ui = boolPool.GetObject(container);
                ui.Set(exposedBool);
                active.Add(ui);
            }
            else if (element is ExposedString exposedString)
            {
                var ui = stringPool.GetObject(container);
                ui.Set(exposedString);
                active.Add(ui);
            }
            else if (element is ExposedObject exposedObject)
            {
                var ui = objectPool.GetObject(container);
                ui.Set(exposedObject);
                active.Add(ui);
            }
            else if (element is ExposedAsset exposedAsset)
            {
                var ui = assetPool.GetObject(container);
                ui.Set(exposedAsset);
                active.Add(ui);
            }
            else if (element is ExposedAnchor exposedAnchor)
            {
                var ui = anchorPool.GetObject(container);
                ui.Set(exposedAnchor);
                active.Add(ui);
            }
        }
        inspecting = inspectable;
        inspecting.onInspectorChange += Reload;
    }
    void Reload()
    {
        if (inspecting != null) Inspect(inspecting);
    }
    void Clear()
    {
        foreach (var ui in active)
        {
            if (ui is InspectorUIButton btn) buttonPool.ReleaseObject(btn);
            else if (ui is InspectorUIVector2 v2) vector2Pool.ReleaseObject(v2);
            else if (ui is InspectorUINumber n) numberPool.ReleaseObject(n);
            else if (ui is InspectorUIBool b) boolPool.ReleaseObject(b);
            else if (ui is InspectorUIString s) stringPool.ReleaseObject(s);
            else if (ui is InspectorUIObject o) objectPool.ReleaseObject(o);
            else if (ui is InspectorUIAsset a) assetPool.ReleaseObject(a);
            else if (ui is InspectorUIAnchor anchor) anchorPool.ReleaseObject(anchor);
        }
        active.Clear();
        if (inspecting != null) inspecting.onInspectorChange -= Reload;
        inspecting = null;
    }
}
