using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WildcardSnapPoint : SnapPoint, IObjectDraggable, IAssetDraggable, IPointerDownHandler
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] TMP_Dropdown typeDropdown;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Toggle toggle;
    [SerializeField] TMP_Text setObjectText;
    [SerializeField] TMP_Text setAssetText;

    public enum TypeIndex { Number = 0, Condition = 1, String = 2, Object = 3, Asset = 4 }

    MyGameObject setObject;
    MyAsset setAsset;

    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && codeBlock is PropertyCodeBlock;
    }

    private void OnEnable()
    {
        typeDropdown.onValueChanged.AddListener(OnTypeChange);
        OnTypeChange(typeDropdown.value);
    }
    private void OnDisable()
    {
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
        UpdateDefaultUI();
        base.OnSnappedChange();
    }

    void UpdateDefaultUI()
    {
        bool free = snapped == null;
        typeDropdown.gameObject.SetActive(free);
        var idx = (TypeIndex)typeDropdown.value;
        inputField.gameObject.SetActive(free && (idx == TypeIndex.Number || idx == TypeIndex.String));
        toggle.gameObject.SetActive(free && idx == TypeIndex.Condition);
        setObjectText.gameObject.SetActive(free && idx == TypeIndex.Object);
        setAssetText.gameObject.SetActive(free && idx == TypeIndex.Asset);
    }

    private void Update()
    {
        if(layoutElement != null) layoutElement.minWidth = GetWidth();
    }

    public float GetNumber(ulong hash)
    {
        if (snapped is PropertyCodeBlock p && (p.propertyType & PropertyType.Number) > 0)
            return p.GetNumber(hash);
        if (snapped == null && float.TryParse(inputField.text, out float v))
            return v;
        return 0f;
    }

    public bool GetCondition(ulong hash)
    {
        if (snapped is PropertyCodeBlock p && (p.propertyType & PropertyType.Condition) > 0)
            return p.GetCondition(hash);
        if (snapped == null) return toggle.isOn;
        return false;
    }

    public string GetString(ulong hash)
    {
        if (snapped is PropertyCodeBlock p && (p.propertyType & PropertyType.String) > 0)
            return p.GetString(hash);
        if (snapped == null) return inputField.text;
        return string.Empty;
    }

    public MyGameObject GetObject(ulong hash)
    {
        if (snapped is PropertyCodeBlock p && (p.propertyType & PropertyType.Object) > 0)
            return p.GetObject(hash);
        if (snapped == null) return setObject;
        return null;
    }

    public MyAsset GetAsset(ulong hash)
    {
        if (snapped is PropertyCodeBlock p && (p.propertyType & PropertyType.Asset) > 0)
            return p.GetAsset(hash);
        if (snapped == null) return setAsset;
        return null;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used || snapped != null) return;
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            var idx = (TypeIndex)typeDropdown.value;
            if (idx == TypeIndex.Object) { eventData.Use(); SetObject(null); }
            else if (idx == TypeIndex.Asset) { eventData.Use(); SetAsset(null); }
        }
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
