using UnityEngine;
using UnityEngine.UI;

// A typed snap point that accepts only color-producing property blocks, or a
// literal color entered via the shared ColorMenu (swatch button). Mirrors
// Vector2SnapPoint.
public class ColorSnapPoint : SnapPoint, IColorMenuUser
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    [SerializeField] Button colorButton;
    [SerializeField] Image colorSwatch;

    Color setColor = UnityEngine.Color.white;

    public Color color => setColor;
    public void SetColor(Color color)
    {
        setColor = color;
        if (colorSwatch != null) colorSwatch.color = color;
    }

    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) &&
            codeBlock is PropertyCodeBlock propertyBlock &&
            (propertyBlock.propertyType & PropertyType.Color) > 0;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (colorButton != null) colorButton.onClick.AddListener(OpenColorMenu);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (colorButton != null) colorButton.onClick.RemoveListener(OpenColorMenu);
        var menu = EditorSceneManager.Instance.colorMenu;
        if (menu.currentUser == (IColorMenuUser)this) menu.Close();
    }
    void OpenColorMenu()
    {
        if (snapped != null) return;
        var menu = EditorSceneManager.Instance.colorMenu;
        menu.Open(this);
        menu.transform.position = colorButton.transform.position;
    }
    protected override void OnSnappedChange()
    {
        unSnapped.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    private void Update()
    {
        if (layoutElement != null) layoutElement.minWidth = GetWidth();
    }
    public Color GetColor(ulong hash)
    {
        if (snapped is PropertyCodeBlock propertyBlock) return propertyBlock.GetColor(hash);
        return setColor;
    }
    public float GetWidth()
    {
        if (snapped is PropertyCodeBlock propertyBlock) return propertyBlock.GetWidth();
        return defaultWidth;
    }
    public override void Clear()
    {
        base.Clear();
        SetColor(UnityEngine.Color.white);
    }
    public override SnapPointSave Save()
    {
        var save = base.Save();
        save.data.floats["colorR"] = setColor.r;
        save.data.floats["colorG"] = setColor.g;
        save.data.floats["colorB"] = setColor.b;
        save.data.floats["colorA"] = setColor.a;
        return save;
    }
    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        float cr = save.data.floats.TryGetValue("colorR", out float r2) ? r2 : 1f;
        float cg = save.data.floats.TryGetValue("colorG", out float g2) ? g2 : 1f;
        float cb = save.data.floats.TryGetValue("colorB", out float b2) ? b2 : 1f;
        float ca = save.data.floats.TryGetValue("colorA", out float a2) ? a2 : 1f;
        SetColor(new Color(cr, cg, cb, ca));
    }
}
