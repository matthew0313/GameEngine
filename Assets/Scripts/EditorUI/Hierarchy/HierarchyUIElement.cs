using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] TMP_InputField renameInput;
    [SerializeField] Color idleColor, selectedColor, movingColor;

    public MyGameObject target { get; private set; }
    bool folded { get => target != null && target.foldedInInspector; set { if (target != null) target.foldedInInspector = value; } }
    readonly List<HierarchyUIElement> childElements = new();
    private void OnEnable()
    {
        foldoutButton.onClick.AddListener(ToggleFoldout);
        renameInput.onEndEdit.AddListener(OnRenameEnd);
    }
    private void OnDisable()
    {
        foldoutButton.onClick.RemoveListener(ToggleFoldout);
        renameInput.onEndEdit.RemoveListener(OnRenameEnd);
    }
    HierarchyUI origin;
    Pooler<HierarchyUIElement> elementPool;
    public void Init(HierarchyUI origin, Pooler<HierarchyUIElement> pool)
    {
        this.origin = origin;
        elementPool = pool;
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
    private void Update()
    {
        if(target == origin.moving) background.color = movingColor;
        else if (EditorSceneManager.Instance.selected != null && EditorSceneManager.Instance.selected == target) background.color = selectedColor;
        else background.color = idleColor;
    }
    void OnPropertyChange()
    {
        nameText.text = target.name;
        icon.sprite = target.icon;
    }
    void RefreshChildren()
    {
        bool hasChild = false; int i = 0;
        foreach (var child in target.GetChildren())
        {
            hasChild = true;
            if (childElements.Count <= i) childElements.Add(elementPool.GetObject(childrenContainer.transform));
            childElements[i].gameObject.SetActive(true);
            childElements[i++].Set(child);
        }
        for (; i < childElements.Count; i++)
        {
            elementPool.ReleaseObject(childElements[i]);
            childElements.RemoveAt(i--);
        }
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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.Use();
            if (origin.moving != null) origin.Reparent(target);
            else EditorSceneManager.Instance.Select(target);
        }
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            eventData.Use();
            origin.EnterMoveMode(target);
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.Use();
            origin.RightClickMenu(this);
        }
    }
    public void Rename()
    {
        renameInput.text = target.name;
        renameInput.gameObject.SetActive(true);
        renameInput.Select();
    }
    void OnRenameEnd(string text)
    {
        renameInput.gameObject.SetActive(false);
        target.name = text;
        target.OnPropertyChange();
    }
}
