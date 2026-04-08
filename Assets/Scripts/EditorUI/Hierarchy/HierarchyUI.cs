using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchyUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Transform elementAnchor;
    [SerializeField] HierarchyUIElement elementPrefab;

    readonly List<HierarchyUIElement> elements = new();
    Pooler<HierarchyUIElement> elementPool;

    InputOverride movingDownKeyOverride, movingUpKeyOverride, movingESCOverride;
    private void Awake()
    {
        elementPool = new(() =>
        {
            var element = Instantiate(elementPrefab);
            element.Init(this, elementPool);
            return element;
        });
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
    public void EnterMoveMode(MyGameObject obj)
    {
        if (moving != null)
        {
            moving = obj; return;
        }
        moving = obj;
        InputManager.Instance.AddOverride(KeyCode.DownArrow, movingDownKeyOverride);
        InputManager.Instance.AddOverride(KeyCode.UpArrow, movingUpKeyOverride);
        InputManager.Instance.AddOverride(KeyCode.Escape, movingESCOverride);
    }
    public void Reparent(MyGameObject newParent)
    {
        if (moving == null || newParent == moving) return;
        if(newParent == null)
        {
            EditorSceneManager.Instance.myScene.AddChild(moving);
            ExitMoveMode();
        }
        else if (!newParent.IsChildOf(moving))
        {
            newParent.foldedInInspector = false;
            newParent.AddChild(moving);
            ExitMoveMode();
        }
    }
    void OnMovingDownKey()
    {
        if (moving == null || moving.parent == null) return;
        moving.SetSiblingIndex(moving.GetSiblingIndex() + 1);
    }
    void OnMovingUpKey()
    {
        if (moving == null || moving.parent == null) return;
        moving.SetSiblingIndex(moving.GetSiblingIndex() - 1);
    }
    void OnMovingESC()
    {
        ExitMoveMode();
    }
    void ExitMoveMode()
    {
        if (moving == null) return;
        moving = null;
        InputManager.Instance.RemoveOverride(KeyCode.DownArrow, movingDownKeyOverride);
        InputManager.Instance.RemoveOverride(KeyCode.UpArrow, movingUpKeyOverride);
        InputManager.Instance.RemoveOverride(KeyCode.Escape, movingESCOverride);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if (eventData.button == PointerEventData.InputButton.Left && moving != null) Reparent(null);
    }
}