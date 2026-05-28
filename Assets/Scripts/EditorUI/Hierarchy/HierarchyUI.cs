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
    public void ExitMoveMode()
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
        if (eventData.button == PointerEventData.InputButton.Left && moving != null)
        {
            eventData.Use();
            Reparent(null);
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.Use();
            RightClickMenu(null);
        }
    }
    public void RightClickMenu(HierarchyUIElement element)
    {
        EditorSceneManager.Instance.rightClickMenu.Open(Input.mousePosition, MakeRightClickMenu(element));
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu(HierarchyUIElement element)
    {
        if (element != null)
        {
            yield return new RCMenuElement_Button(
                "Move To Object",
                ctx =>
                {
                    EditorSceneManager.Instance.sceneScreen.MoveTo(element.target.transform.position, true);
                });
            yield return new RCMenuElement_Button(
                "Make Prefab",
                ctx =>
                {
                    PrefabAsset tmp = new();
                    tmp.Set(element.target);
                    EditorSceneManager.Instance.AddAsset(tmp);
                });
            yield return new RCMenuElement_Button(
                "Rename",
                ctx =>
                {
                    element.Rename();
                });
            yield return new RCMenuElement_Button(
                "Delete",
                ctx =>
                {
                    element.target.Delete();
                });
        }
        yield return new RCMenuElement_Foldout(
            element != null ? "Create Child Object" : "Create Object",
            MakeRightClickMenu_CreateObject(element));
    }
    void CreateObject(HierarchyUIElement element, MyGameObject obj)
    {
        MyGameObject newObj = Instantiate(obj);
        Vector3 localScale = newObj.transform.localScale;
        if (element == null)
        {
            int index = 0;
            while (true)
            {
                if (EditorSceneManager.Instance.myScene.GetChildren().Find(item => item.name == obj.name + (index > 0 ? $"{index}" : ""))) index++;
                else break;
            }
            newObj.name = obj.name + (index > 0 ? $"{index}" : "");
            EditorSceneManager.Instance.myScene.AddChild(newObj);
        }
        else
        {
            int index = 0;
            while (true)
            {
                if (element.target.GetChildren().Find(item => item.name == obj.name + (index > 0 ? $"{index}" : ""))) index++;
                else break;
            }
            newObj.name = obj.name + (index > 0 ? $"{index}" : "");
            element.target.AddChild(newObj);
        }
        newObj.transform.localScale = localScale;
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu_CreateObject(HierarchyUIElement element)
    {
        bool UI = false;
        foreach(var obj in EditorSceneManager.Instance.myGameObjectList.myGameObjects)
        {
            if (obj is MyGameObject_UI)
            {
                UI = true; break;
            }
            yield return new RCMenuElement_Button(
                obj.name,
                ctx => CreateObject(element, obj));
        }
        if(UI) yield return new RCMenuElement_Foldout(
            "UI Objects",
            MakeRightClickMenu_CreateObject_UI(element));
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu_CreateObject_UI(HierarchyUIElement element)
    {
        foreach (var obj in EditorSceneManager.Instance.myGameObjectList.myGameObjects)
        {
            if (!(obj is MyGameObject_UI)) continue;
            yield return new RCMenuElement_Button(
                obj.name,
                ctx => CreateObject(element, obj));
        }
    }
}