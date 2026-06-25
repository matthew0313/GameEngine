using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class WildcardSnapPoint : SnapPoint, IObjectDraggable, IAssetDraggable, IPointerDownHandler, IColorMenuUser
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    [SerializeField] TMP_Dropdown typeDropdown;
    [SerializeField] TMP_InputField inputField, vectorXInput, vectorYInput;
    [SerializeField] Toggle toggle;
    [SerializeField] TMP_Text setObjectText;
    [SerializeField] TMP_Text setAssetText;
    [SerializeField] Button colorButton;
    [SerializeField] Image colorSwatch;

    public enum TypeIndex { Number = 0, Condition = 1, String = 2, Object = 3, Asset = 4, Vector2 = 5, Color = 6, Array = 7 }

    MyGameObject setObject;
    MyAsset setAsset;
    Color setColor = UnityEngine.Color.white;

    public Color color => setColor;
    public void SetColor(Color color)
    {
        setColor = color;
        if (colorSwatch != null) colorSwatch.color = color;
    }

    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) && codeBlock is PropertyCodeBlock;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        typeDropdown.onValueChanged.AddListener(OnTypeChange);
        if (colorButton != null) colorButton.onClick.AddListener(OpenColorMenu);
        OnTypeChange(typeDropdown.value);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        typeDropdown.onValueChanged.RemoveListener(OnTypeChange);
        if (colorButton != null) colorButton.onClick.RemoveListener(OpenColorMenu);
        var menu = EditorSceneManager.Instance != null ? EditorSceneManager.Instance.colorMenu : null;
        if (menu != null && menu.currentUser == (IColorMenuUser)this) menu.Close();
    }
    void OpenColorMenu()
    {
        if (snapped != null) return;
        var menu = EditorSceneManager.Instance.colorMenu;
        menu.Open(this);
        menu.transform.position = colorButton.transform.position;
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
        if (colorButton != null) colorButton.gameObject.SetActive(idx == TypeIndex.Color);
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
    public Vector2 GetVector2(ulong hash)
    {
        if (snapped is PropertyCodeBlock p) return p.GetVector2(hash);
        Vector2 tmp = new();
        float.TryParse(vectorXInput.text, out tmp.x);
        float.TryParse(vectorYInput.text, out tmp.y);
        return tmp;
    }
    public Color GetColor(ulong hash)
    {
        if (snapped is PropertyCodeBlock p) return p.GetColor(hash);
        return setColor;
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
            vector2 = GetVector2(hash),
            color = GetColor(hash),
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
        SetColor(UnityEngine.Color.white);
    }

    public override SnapPointSave Save()
    {
        var save = base.Save();
        save.data.integers["typeIndex"] = typeDropdown.value;
        save.data.strings["inputValue"] = inputField.text;
        save.data.floats["vectorXInput"] = float.TryParse(vectorXInput.text, out float x) ? x : 0.0f;
        save.data.floats["vectorYInput"] = float.TryParse(vectorYInput.text, out float y) ? y : 0.0f;
        save.data.bools["toggleValue"] = toggle.isOn;
        save.data.ulongs["setObject"] = setObject != null ? setObject.uid : 0;
        save.data.ulongs["setAsset"] = setAsset != null ? setAsset.uid : 0;
        save.data.floats["colorR"] = setColor.r;
        save.data.floats["colorG"] = setColor.g;
        save.data.floats["colorB"] = setColor.b;
        save.data.floats["colorA"] = setColor.a;
        return save;
    }

    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        if (save.data.integers.TryGetValue("typeIndex", out int typeIndex)) typeDropdown.value = typeIndex;
        if (save.data.strings.TryGetValue("inputValue", out string inputValue)) inputField.text = inputValue;
        if (save.data.floats.TryGetValue("vectorXInput", out float vectorX)) vectorXInput.text = vectorX.ToString();
        if (save.data.floats.TryGetValue("vectorYInput", out float vectorY)) vectorYInput.text = vectorY.ToString();
        if (save.data.bools.TryGetValue("toggleValue", out bool toggleValue)) toggle.isOn = toggleValue;
        if (save.data.ulongs.TryGetValue("setObject", out ulong setObjectId)) SetObject(EditorSceneManager.Instance.FindObjectWithUID(setObjectId));
        if (save.data.ulongs.TryGetValue("setAsset", out ulong setAssetId)) SetAsset(EditorSceneManager.Instance.GetAsset<MyAsset>(setAssetId));
        float cr = save.data.floats.TryGetValue("colorR", out float r2) ? r2 : 1f;
        float cg = save.data.floats.TryGetValue("colorG", out float g2) ? g2 : 1f;
        float cb = save.data.floats.TryGetValue("colorB", out float b2) ? b2 : 1f;
        float ca = save.data.floats.TryGetValue("colorA", out float a2) ? a2 : 1f;
        SetColor(new Color(cr, cg, cb, ca));
    }
}