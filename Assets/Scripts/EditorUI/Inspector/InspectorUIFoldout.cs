using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUIFoldout : InspectorUIElement
{
    [SerializeField] TMP_Text label;
    [SerializeField] Transform container;
    [SerializeField] GameObject open, close;
    [SerializeField] Button foldoutButton;
    ExposedFoldout element;
    readonly List<InspectorUIElement> active = new();
    bool folded = false;
    private void OnEnable()
    {
        foldoutButton.onClick.AddListener(ToggleFoldout);
    }
    void ToggleFoldout() => SetFolded(!folded);
    void SetFolded(bool folded)
    {
        this.folded = folded;
        open.SetActive(!folded);
        close.SetActive(folded);
        container.gameObject.SetActive(!folded);
        data.bools[element.name + "_folded"] = folded;
    }
    public void Set(ExposedFoldout element)
    {
        this.element = element;
        label.text = element.name;
        foreach (var child in element.elements)
        {
            if (!child.visible) continue;
            var ui = inspectorUI.GetElement(child, container);
            active.Add(ui);
        }
        if (data.bools.TryGetValue(element.name + "_folded", out folded)) SetFolded(folded);
        else SetFolded(true);
    }
    private void OnDisable()
    {
        foldoutButton.onClick.RemoveListener(ToggleFoldout);
        foreach (var ui in active) inspectorUI.ReleaseElement(ui);
        active.Clear();
    }
}
