using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchyUI : MonoBehaviour
{
    [SerializeField] Transform elementAnchor;
    [SerializeField] HierarchyUIElement elementPrefab;

    readonly List<HierarchyUIElement> elements = new();

    private void Awake()
    {
        EditorSceneManager.Instance.myScene.onHierarchyChange += Refresh;
    }
    private void Start()
    {
        Refresh();
    }
    public void Refresh()
    {
        MyScene scene = EditorSceneManager.Instance.myScene;
        int i = 0;
        foreach (var obj in scene.topGameObjects)
        {
            if (elements.Count <= i)
                elements.Add(Instantiate(elementPrefab, elementAnchor));
            elements[i].gameObject.SetActive(true);
            elements[i++].Set(obj);
        }
        for (; i < elements.Count; i++)
            elements[i].gameObject.SetActive(false);
    }
}