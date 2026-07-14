using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorMenu : MonoBehaviour, IDragHandler
{
    [Header("Preview")]
    [SerializeField] Image colorImage;

    [Header("Spectrum")]
    [SerializeField] Image svImage;
    [SerializeField] ColorPickArea svArea;
    [SerializeField] RectTransform svHandle;
    [SerializeField] Image hueImage;
    [SerializeField] ColorPickArea hueArea;
    [SerializeField] RectTransform hueHandle;
    [SerializeField] Image rBarImage, gBarImage, bBarImage, aBarImage;

    [Header("Channels")]
    [SerializeField] Slider rSlider;
    [SerializeField] Slider gSlider, bSlider, aSlider;
    [SerializeField] TMP_InputField rField, gField, bField, aField;

    public IColorMenuUser currentUser { get; private set; }

    Color color;
    float h, s, v;
    bool updating;
    const int texSize = 64;
    Texture2D svTex;
    bool texturesBuilt;

    static InputOverride exitMenuInput;
    bool openedFrame;

    // Open the shared menu for a user. The caller is responsible for positioning
    // (set this.transform.position right after calling Open).
    public void Open(IColorMenuUser user)
    {
        currentUser = user;
        gameObject.SetActive(true);
        openedFrame = true;
        SetColor(user.color);
        exitMenuInput ??= new() { priority = 9999, onTrigger = Close };
        InputManager.Instance.AddOverride(KeyCode.Escape, exitMenuInput);
    }
    public void Close()
    {
        if (!gameObject.activeSelf) return;
        InputManager.Instance.RemoveOverride(KeyCode.Escape, exitMenuInput);
        currentUser = null;
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        BuildTextures();
        rSlider.onValueChanged.AddListener(OnSliderChange);
        gSlider.onValueChanged.AddListener(OnSliderChange);
        bSlider.onValueChanged.AddListener(OnSliderChange);
        aSlider.onValueChanged.AddListener(OnSliderChange);
        rField.onValueChanged.AddListener(OnFieldChange);
        gField.onValueChanged.AddListener(OnFieldChange);
        bField.onValueChanged.AddListener(OnFieldChange);
        aField.onValueChanged.AddListener(OnFieldChange);
        svArea.onPick += OnSVPick;
        hueArea.onPick += OnHuePick;
    }
    private void OnDisable()
    {
        rSlider.onValueChanged.RemoveListener(OnSliderChange);
        gSlider.onValueChanged.RemoveListener(OnSliderChange);
        bSlider.onValueChanged.RemoveListener(OnSliderChange);
        aSlider.onValueChanged.RemoveListener(OnSliderChange);
        rField.onValueChanged.RemoveListener(OnFieldChange);
        gField.onValueChanged.RemoveListener(OnFieldChange);
        bField.onValueChanged.RemoveListener(OnFieldChange);
        aField.onValueChanged.RemoveListener(OnFieldChange);
        svArea.onPick -= OnSVPick;
        hueArea.onPick -= OnHuePick;
    }
    private void Update()
    {
        // close if the user went away (e.g. inspector cleared / block deleted)
        if (currentUser == null || (currentUser is Object o && o == null))
        {
            Close();
            return;
        }
        Color color = currentUser.color;
        if (color != this.color) SetColor(color);

        // click-outside closes (skip the frame we opened on to avoid self-close)
        var tmp = UIScanner.ScanUI(Input.mousePosition);
        if (!openedFrame
            && Input.GetMouseButtonDown(0) &&
            (tmp.Count > 0 && !tmp[0].gameObject.transform.IsChildOf(transform)))
        {
            Close();
        }
        else openedFrame = false;
    }

    // --- spectrum input ---
    void OnSVPick(Vector2 n)
    {
        s = n.x;
        v = n.y;
        Color color = Color.HSVToRGB(h, s, v);
        color.a = this.color.a;
        SetColor(color);
    }
    void OnHuePick(Vector2 n)
    {
        h = Mathf.Clamp01(n.y);
        Color color = Color.HSVToRGB(h, s, v);
        color.a = this.color.a;
        SetColor(color);
    }

    // --- channel input ---
    void OnSliderChange(float _)
    {
        if (updating) return;
        SetColor(new Color32((byte)rSlider.value, (byte)gSlider.value, (byte)bSlider.value, (byte)aSlider.value));
    }
    void OnFieldChange(string _)
    {
        if (updating) return;
        int.TryParse(rField.text, out int r);
        int.TryParse(gField.text, out int g);
        int.TryParse(bField.text, out int b);
        int.TryParse(aField.text, out int a);
        SetColor(new Color32((byte)r, (byte)g, (byte)b, (byte)a));
    }

    // Adopt an RGBA color (from outside or the channel controls).
    void SetColor(Color color)
    {
        this.color = color;
        currentUser?.SetColor(color);
        Color.RGBToHSV(color, out h, out s, out v);
        BuildSVTexture();
        BuildBarTexture();
        Refresh(color);
    }
    // Push the resolved color out to every control + the inspected element.
    void Refresh(Color color)
    {
        updating = true;
        colorImage.color = color;
        Color32 color32 = color;
        rSlider.value = color32.r;
        gSlider.value = color32.g;
        bSlider.value = color32.b;
        aSlider.value = color32.a;
        rField.text = color32.r.ToString();
        gField.text = color32.g.ToString();
        bField.text = color32.b.ToString();
        aField.text = color32.a.ToString();
        UpdateHandles();
        updating = false;
    }

    void UpdateHandles()
    {
        Rect r = ((RectTransform)svImage.transform).rect;
        svHandle.anchoredPosition = new Vector2((s - 0.5f) * r.width, (v - 0.5f) * r.height);

        r = ((RectTransform)hueImage.transform).rect;
        hueHandle.anchoredPosition = new Vector2(0f, (h - 0.5f) * r.height);
    }

    void BuildTextures()
    {
        if (texturesBuilt) return;
        BuildHueTexture();
        BuildBarTexture();
        BuildSVTexture();
        texturesBuilt = true;
    }
    void BuildSVTexture()
    {
        Color.RGBToHSV(color, out h, out s, out v);
        if (svTex == null)
        {
            svTex = new Texture2D(texSize, texSize) { wrapMode = TextureWrapMode.Clamp };
            svImage.sprite = Sprite.Create(svTex, new Rect(0, 0, texSize, texSize), new Vector2(0.5f, 0.5f));
            svImage.type = Image.Type.Simple;
            svImage.color = Color.white;
        }
        for (int y = 0; y < texSize; y++)
            for (int x = 0; x < texSize; x++)
                svTex.SetPixel(x, y, Color.HSVToRGB(h, x / (float)(texSize - 1), y / (float)(texSize - 1)));
        svTex.Apply();
    }
    Texture2D rBarTex, gBarTex, bBarTex, aBarTex;
    bool barTexInit = false;
    void BuildBarTexture()
    {
        if (!barTexInit)
        {
            rBarTex = new Texture2D(texSize, 1) { wrapMode = TextureWrapMode.Clamp };
            rBarImage.sprite = Sprite.Create(rBarTex, new Rect(0, 0, texSize, 1), new Vector2(0.5f, 0.5f));
            gBarTex = new Texture2D(texSize, 1) { wrapMode = TextureWrapMode.Clamp };
            gBarImage.sprite = Sprite.Create(gBarTex, new Rect(0, 0, texSize, 1), new Vector2(0.5f, 0.5f));
            bBarTex = new Texture2D(texSize, 1) { wrapMode = TextureWrapMode.Clamp };
            bBarImage.sprite = Sprite.Create(bBarTex, new Rect(0, 0, texSize, 1), new Vector2(0.5f, 0.5f));
            aBarTex = new Texture2D(texSize, 1) { wrapMode = TextureWrapMode.Clamp };
            aBarImage.sprite = Sprite.Create(aBarTex, new Rect(0, 0, texSize, 1), new Vector2(0.5f, 0.5f));
            barTexInit = true;
        }
        for(int x = 0; x < texSize; x++)
        {
            rBarTex.SetPixel(x, 0, new Color(x / (float)(texSize - 1), color.g, color.b));
            gBarTex.SetPixel(x, 0, new Color(color.r, x / (float)(texSize - 1), color.b));
            bBarTex.SetPixel(x, 0, new Color(color.r, color.g, x / (float)(texSize - 1)));
            aBarTex.SetPixel(x, 0, new Color(color.r, color.g, color.b, x / (float)(texSize - 1)));
        }
        rBarTex.Apply(); gBarTex.Apply(); bBarTex.Apply(); aBarTex.Apply();
    }
    void BuildHueTexture()
    {
        var t = new Texture2D(1, texSize) { wrapMode = TextureWrapMode.Clamp };
        for (int y = 0; y < texSize; y++) t.SetPixel(0, y, Color.HSVToRGB(y / (float)(texSize - 1), 1f, 1f));
        t.Apply();
        hueImage.sprite = Sprite.Create(t, new Rect(0, 0, 1, texSize), new Vector2(0.5f, 0.5f));
        hueImage.type = Image.Type.Simple;
        hueImage.color = Color.white;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.used) return;
        Debug.Log("darg");
        transform.position += (Vector3)eventData.delta;
    }
}
