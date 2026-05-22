using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIAnchorMenu : InspectorUIElement
{
    InspectorUIAnchor UIAnchor;
    public void SetAnchor(int presetIndex)
    {
        if (UIAnchor == null) return;
        UIAnchor.SetAnchor(presetIndex);
    }
    public void Show(InspectorUIAnchor UIAnchor)
    {
        this.UIAnchor = UIAnchor;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
