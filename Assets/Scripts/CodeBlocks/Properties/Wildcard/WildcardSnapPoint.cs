using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class WildcardSnapPoint : SnapPoint, IObjectDraggable, IAssetDraggable, IPointerDownHandler
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    [SerializeField] TMP_Dropdown typeDropdown;
    [SerializeField] TMP_InputField inputField, vectorXInput, vectorYInput;
    [SerializeField] Toggle toggle;
    [SerializeField] TMP_Text setObjectText;
    [SerializeField] TMP_Text setAssetText;

    public enum TypeIndex { Number = 0, Condition = 1, String = 2, Object = 3, Asset = 4, Vector2 = 5, Array = 6 }

    MyGameObject setObject;
    MyAsset setAsset;

    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && codeBlock is PropertyCodeBlock;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        typeDropdown.onValueChanged.AddListener(OnTypeChange);
        OnTypeChange(typeDropdown.value);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        typeDropdown.onValueChanged.RemoveListener(OnTypeChange);
    }
    public void SetType(TypeIndex type)
    {
        typeDropdown.value = (int)type;
        OnTypeChange((int)type);
    }
    void OnTypeChange(int type) => UpdateDefaultUI();
    protected override void OnSnappedChange()
    {
        unSnapped.SetActive(snapped == null);
        UpdateDefaultUI();
        base.OnSnappedChange();
    }

    void UpdateDefaultUI()
    {
        var idx = (TypeIndex)typeDropdown.value;
        inputField.gameObject.SetActive(idx == TypeIndex.Number || idx == TypeIndex.String);
        toggle.gameObject.SetActive(idx == TypeIndex.Condition);
        setObjectText.gameObject.SetActive(idx == TypeIndex.Object);
        setAssetText.gameObject.SetActive(idx == TypeIndex.Asset);
        vectorXInput.gameObject.SetActive(idx == TypeIndex.Vector2);
        vectorYInput.gameObject.SetActive(idx == TypeIndex.Vector2);
    }

    private void Update()
    {
        if(layoutElement != null) layoutElement.minWidth = GetWidth();
    }

    public float GetNumber(ulong hash)
    {
        if (snapped is PropertyCodeBlock p) return p.GetNumber(hash);
        if (snapped == null && float.TryParse(inputField.text, out float v))
            return v;
        return 0f;
    }
    public Vector2 GetVector2(ulong hash)
    {
        if (snapped is PropertyCodeBlock p) return p.GetVector2(hash);
        if (snapped == null &&
            float.TryParse(vectorXInput.text, out float x) &&
            float.TryParse(vectorYInput.text, out float y)) return new Vector2(x, y);
        return new();
    }

    public bool GetCondition(ulong hash)
    {
        if (snapped is PropertyCodeBlock p) return p.GetCondition(hash);
        if (snapped == null) return toggle.isOn;
        return false;
    }

    public string GetString(ulong hash)
    {
        if (snapped is PropertyCodeBlock p) return p.GetString(hash);
        if (snapped == null) return inputField.text;
        return string.Empty;
    }

    public MyGameObject GetObject(ulong hash)
    {
        if (snapped is PropertyCodeBlock p) return p.GetObject(hash);
        if (snapped == null) return setObject;
        return null;
    }

    public MyAsset GetAsset(ulong hash)
    {
        if (snapped is PropertyCodeBlock p) return p.GetAsset(hash);
        if (snapped == null) return setAsset;
        return null;
    }
    public Wildcard GetWildcard(ulong hash)
    {
        return new()
        {
            number = GetNumber(hash),
            condition = GetCondition(hash),
            str = GetString(hash),
            obj = GetObject(hash),
            asset = GetAsset(hash),
        };
    }

    public float GetWidth()
    {
        if (snapped is PropertyCodeBlock p) return p.GetWidth();
        return defaultWidth;
    }

    public void OnObjectDrag(MyGameObject obj)
    {
        if (snapped != null || (TypeIndex)typeDropdown.value != TypeIndex.Object) return;
        SetObject(obj);
    }

    void SetObject(MyGameObject obj)
    {
        if (setObject != null)
        {
            setObject.onDelete -= OnSetObjectDelete;
            setObject.onDisplayChange -= OnSetObjectDisplayChange;
        }
        setObject = obj;
        OnSetObjectDisplayChange();
        if (setObject != null)
        {
            setObject.onDelete += OnSetObjectDelete;
            setObject.onDisplayChange += OnSetObjectDisplayChange;
        }
    }

    void OnSetObjectDelete() => SetObject(null);
    void OnSetObjectDisplayChange() => setObjectText.text = setObject != null ? setObject.name : "None";

    public void OnAssetDrag(MyAsset asset)
    {
        if (snapped != null || (TypeIndex)typeDropdown.value != TypeIndex.Asset) return;
        SetAsset(asset);
    }

    void SetAsset(MyAsset asset)
    {
        setAsset = asset;
        setAssetText.text = setAsset != null ? setAsset.name : "None";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used || snapped != null) return;
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            var idx = (TypeIndex)typeDropdown.value;
            if (idx == TypeIndex.Object) { eventData.Use(); SetObject(null); }
            else if (idx == TypeIndex.Asset) { eventData.Use(); SetAsset(null); }
        }
    }
    public override void Clear()
    {
        base.Clear();
        typeDropdown.value = 0;
        inputField.text = string.Empty;
        toggle.isOn = false;
        SetObject(null); SetAsset(null);
    }

    public override SnapPointSave Save()
    {
        var save = base.Save();
        save.data.integers["typeIndex"] = typeDropdown.value;
        save.data.strings["inputValue"] = inputField.text;
        save.data.bools["toggleValue"] = toggle.isOn;
        save.data.ulongs["setObject"] = setObject != null ? setObject.uid : 0;
        save.data.ulongs["setAsset"] = setAsset != null ? setAsset.uid : 0;
        return save;
    }

    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        typeDropdown.value = save.data.integers["typeIndex"];
        inputField.text = save.data.strings["inputValue"];
        toggle.isOn = save.data.bools["toggleValue"];
        SetObject(EditorSceneManager.Instance.FindObjectWithUID(save.data.ulongs["setObject"]));
        SetAsset(EditorSceneManager.Instance.GetAsset<MyAsset>(save.data.ulongs["setAsset"]));
    }
}