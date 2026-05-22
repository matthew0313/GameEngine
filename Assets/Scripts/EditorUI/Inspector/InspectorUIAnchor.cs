using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIAnchor : InspectorUIElement
{
    [SerializeField] Button openMenuButton;
    [SerializeField] Transform menuAnchor;
    [SerializeField] InspectorUIAnchorMenu menu;
    [SerializeField] Image currentPresetIcon;
    [SerializeField] AnchorPreset[] presets;

    bool menuOpen = false;
    ExposedAnchor element;
    Vector2 anchorMin, anchorMax;

    static InputOverride exitMenuInput;
    public void Set(ExposedAnchor element)
    {
        this.element = element;
        anchorMin = element.minGetter();
        anchorMax = element.maxGetter();
        currentPresetIcon.sprite = null;
        foreach (var preset in presets)
        {
            if (preset.anchorMin == anchorMin && preset.anchorMax == anchorMax)
            {
                currentPresetIcon.sprite = preset.icon;
                break;
            }
        }
    }
    public void SetAnchor(int presetIndex)
    {
        if (element == null) return;
        if (presetIndex < 0 || presetIndex >= presets.Length) return;
        var preset = presets[presetIndex];
        element.minSetter(preset.anchorMin);
        element.maxSetter(preset.anchorMax);
    }
    private void OnEnable()
    {
        openMenuButton.onClick.AddListener(OpenMenu);
    }
    private void OnDisable()
    {
        openMenuButton.onClick.RemoveListener(OpenMenu);
        CloseMenu();
    }
    void OpenMenu()
    {
        if (menuOpen) return;
        menu.Show(this);
        menu.transform.position = menuAnchor.position;
        menuOpen = true;
        exitMenuInput ??= new() { priority = 9999, onTrigger = CloseMenu };
        InputManager.Instance.AddOverride(KeyCode.Escape, exitMenuInput);
    }
    void CloseMenu()
    {
        if (!menuOpen) return;
        menu.Hide();
        menuOpen = false;
        InputManager.Instance.RemoveOverride(KeyCode.Escape, exitMenuInput);
    }
    private void Update()
    {
        if (element == null) return;
        Vector2 tmpMin = element.minGetter(), tmpMax = element.maxGetter();
        if(tmpMin != anchorMin || tmpMax != anchorMax)
        {
            anchorMin = tmpMin;
            anchorMax = tmpMax;
            currentPresetIcon.sprite = null;
            foreach (var preset in presets)
            {
                if (preset.anchorMin == anchorMin && preset.anchorMax == anchorMax)
                {
                    currentPresetIcon.sprite = preset.icon;
                    break;
                }
            }
        }
        if(menuOpen)
        {
            menu.transform.position = menuAnchor.position;
            if (Input.GetMouseButtonDown(0) && !UIScanner.ScanUI(Input.mousePosition)[0].gameObject.transform.IsChildOf(menu.transform)) CloseMenu();
        }
    }

    [System.Serializable]
    public struct AnchorPreset
    {
        public Vector2 anchorMin, anchorMax;
        public Sprite icon;
    }
}
