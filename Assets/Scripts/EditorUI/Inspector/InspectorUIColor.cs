using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIColor : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] Button openMenuButton;
    [SerializeField] Transform menuAnchor;
    [SerializeField] InspectorUIColorMenu menu;
    [SerializeField] Image colorImage;

    bool menuOpen = false;
    ExposedColor element;
    public Color color => element.getter();

    static InputOverride exitMenuInput;
    public void Set(ExposedColor element)
    {
        this.element = element;
        label.text = element.name;
        colorImage.color = element.getter();
    }
    public void SetColor(Color color)
    {
        if (element == null) return;
        element.setter(color);
        colorImage.color = color;
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
        colorImage.color = element.getter();
        if (menuOpen)
        {
            menu.transform.position = menuAnchor.position;
            if (Input.GetMouseButtonDown(0) && !UIScanner.ScanUI(Input.mousePosition)[0].gameObject.transform.IsChildOf(menu.transform)) CloseMenu();
        }
    }
}
