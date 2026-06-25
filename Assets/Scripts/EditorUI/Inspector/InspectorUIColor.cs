using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIColor : InspectorUIElement, IColorMenuUser
{
    [SerializeField] TMP_Text label;
    [SerializeField] Button openMenuButton;
    [SerializeField] Transform menuAnchor;
    [SerializeField] Image colorImage;

    ExposedColor element;
    public Color color => element.getter();

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
        var menu = EditorSceneManager.Instance != null ? EditorSceneManager.Instance.colorMenu : null;
        if (menu != null && menu.currentUser == (IColorMenuUser)this) menu.Close();
    }
    void OpenMenu()
    {
        var menu = EditorSceneManager.Instance.colorMenu;
        if (menu.currentUser == (IColorMenuUser)this) return;
        menu.Open(this);
        menu.transform.position = menuAnchor.position;
    }
    private void Update()
    {
        colorImage.color = element.getter();
        var menu = EditorSceneManager.Instance.colorMenu;
        if (menu.currentUser == (IColorMenuUser)this) menu.transform.position = menuAnchor.position;
    }
}
