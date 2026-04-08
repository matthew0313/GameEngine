using System.Collections.Generic;
using UnityEngine;

public class HierarchyUI : MonoBehaviour
{
    [SerializeField] Transform elementAnchor;
    [SerializeField] HierarchyUIElement elementPrefab;

    readonly List<HierarchyUIElement> elements = new();
    Pooler<HierarchyUIElement> elementPool;

    InputOverride movingLeftClickOverride, movingDownKeyOverride, movingUpKeyOverride, movingESCOverride;
    private void Awake()
    {
        elementPool = new(() =>
        {
            var element = Instantiate(elementPrefab);
            element.Init(this, elementPool);
            return element;
        });
        movingLeftClickOverride = new()
        {
            priority = 100,
            onTrigger = OnMovingLeftClick
        };
        movingDownKeyOverride = new()
        {
            priority = 100,
            onTrigger = OnMovingDownKey
        };
        movingUpKeyOverride = new()
        {
            priority = 100,
            onTrigger = OnMovingUpKey
        };
        movingESCOverride = new()
        {
            priority = 100,
            onTrigger = OnMovingESC
        };
        EditorSceneManager.Instance.myScene.onHierarchyChange += Refresh;
    }
    private void Start()
    {
        Refresh();
    }
    public MyGameObject moving { get; private set; } = null;
    readonly ControlElement<bool> raycastDisable = new(() => false, 0);
    public void EnterMoveMode(MyGameObject obj)
    {
        if (moving != null) return;
        moving = obj;
        EditorSceneManager.Instance.raycastControl.AddControl(raycastDisable);
        InputManager.Instance.AddOverride(0, movingLeftClickOverride);
        InputManager.Instance.AddOverride(KeyCode.DownArrow, movingDownKeyOverride);
        InputManager.Instance.AddOverride(KeyCode.UpArrow, movingUpKeyOverride);
    }
    void OnMovingLeftClick()
    {
        if (moving == null) return;
        HierarchyUIElement element = null;
        bool hierarchyHit = false;
        foreach (var hit in EditorSceneManager.Instance.RaycastUI(Input.mousePosition))
        {
            if (element == null && hit.gameObject.TryGetComponentInParents(out element)) { if (element == this) element = null; }
            if (hit.gameObject.TryGetComponentInParents(out HierarchyUI hierarchy)) hierarchyHit = true;
        }
        if (element != null)
        {
            MyGameObject other = element.target;
            if (!other.IsChildOf(moving))
            {
                other.foldedInInspector = false;
                moving.parent?.RemoveChild(moving);
                other.AddChild(moving);
            }
        }
        else if (hierarchyHit)
        {
            moving.parent?.RemoveChild(moving);
            EditorSceneManager.Instance.myScene.AddChild(moving);
        }
        ExitMoveMode();
    }
    void OnMovingDownKey()
    {
        if (moving == null || moving.parent == null) return;
        moving.SetSiblingIndex(moving.GetSiblingIndex() + 1);
    }
    void OnMovingUpKey()
    {
        if (moving == null || moving.parent == null) return;
        moving.SetSiblingIndex(-moving.GetSiblingIndex() - 1);
    }
    void OnMovingESC()
    {
        ExitMoveMode();
    }
    void ExitMoveMode()
    {
        if (moving == null) return;
        moving = null;
        EditorSceneManager.Instance.raycastControl.RemoveControl(raycastDisable);
        InputManager.Instance.RemoveOverride(0, movingLeftClickOverride);
        InputManager.Instance.RemoveOverride(KeyCode.DownArrow, movingDownKeyOverride);
        InputManager.Instance.RemoveOverride(KeyCode.UpArrow, movingUpKeyOverride);
    }
    public void Refresh()
    {
        MyScene scene = EditorSceneManager.Instance.myScene;
        int i = 0;
        foreach (var obj in scene.topGameObjects)
        {
            if (elements.Count <= i) elements.Add(elementPool.GetObject(elementAnchor));
            elements[i].gameObject.SetActive(true);
            elements[i++].Set(obj);
        }
        for (; i < elements.Count; i++)
        {
            elementPool.ReleaseObject(elements[i]);
            elements.RemoveAt(i--);
        }
    }
}