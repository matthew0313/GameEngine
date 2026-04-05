using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HierarchyUIElement : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Button foldoutButton;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] GameObject childrenContainer;
    [SerializeField] HierarchyUIElement elementPrefab;
    [SerializeField] Image background;
    [SerializeField] Color idleColor, selectedColor;

    MyGameObject target;
    bool folded = true;
    readonly List<HierarchyUIElement> childElements = new();
    private void OnEnable()
    {
        foldoutButton.onClick.AddListener(ToggleFoldout);
        EditorSceneManager.Instance.onSelect += OnSelect;
        OnSelect(EditorSceneManager.Instance.selected);
    }
    private void OnDisable()
    {
        foldoutButton.onClick.RemoveListener(ToggleFoldout);
        EditorSceneManager.Instance.onSelect -= OnSelect;
    }
    public void Set(MyGameObject obj)
    {
        if (target != null) target.onPropertyChange -= OnPropertyChange;
        target = obj;
        target.onPropertyChange += OnPropertyChange;
        nameText.text = target.name;
        icon.sprite = target.icon;
        childrenContainer.SetActive(!folded);
        RefreshChildren();
    }

    void OnPropertyChange()
    {
        nameText.text = target.name;
        icon.sprite = target.icon;
    }
    void OnSelect(ISelectable selectable)
    {
        background.color = (selectable != null && target == selectable) ? selectedColor : idleColor;
    }
    void RefreshChildren()
    {
        bool hasChild = false; int i = 0;
        foreach (var child in target.GetChildren())
        {
            hasChild = true;
            if (childElements.Count <= i)
                childElements.Add(Instantiate(elementPrefab, childrenContainer.transform));
            childElements[i].gameObject.SetActive(true);
            childElements[i++].Set(child);
        }
        for (; i < childElements.Count; i++)
            childElements[i].gameObject.SetActive(false);
        foldoutButton.gameObject.SetActive(hasChild);
    }

    void ToggleFoldout()
    {
        folded = !folded;
        childrenContainer.SetActive(!folded);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        EditorSceneManager.Instance.Select(target);
    }
}
