using System;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<InputManager>("InputManager"));
        DontDestroyOnLoad(Instance);
    }
    readonly List<InputOverride> leftClickOverrides = new(), rightClickOverrides = new();
    readonly Dictionary<KeyCode, List<InputOverride>> overrides = new();
    public static bool GetMouseButtonDown(int button)
    {
        switch (button)
        {
            case 0: return Input.GetMouseButtonDown(0) && Instance.leftClickOverrides.Count <= 0;
            case 1: return Input.GetMouseButtonDown(1) && Instance.rightClickOverrides.Count <= 0;
            default: return false;
        }
    }
    public static bool GetKeyDown(KeyCode keyCode) => Input.GetKeyDown(keyCode) && !Instance.overrides.ContainsKey(keyCode);
    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && leftClickOverrides.Count > 0) leftClickOverrides[0].onTrigger.Invoke();
        if (Input.GetMouseButtonDown(1) && rightClickOverrides.Count > 0) rightClickOverrides[0].onTrigger.Invoke();
        foreach (var keyCode in overrides.Keys)
        {
            if(Input.GetKeyDown(keyCode) && overrides[keyCode].Count > 0) overrides[keyCode][0].onTrigger.Invoke();
        }
    }
    public void AddOverride(int button, InputOverride inputOverride)
    {
        switch (button)
        {
            case 0: 
                leftClickOverrides.Add(inputOverride);
                leftClickOverrides.Sort((a, b) => b.priority.CompareTo(a.priority)); break;
            case 1: 
                rightClickOverrides.Add(inputOverride);
                rightClickOverrides.Sort((a, b) => b.priority.CompareTo(a.priority)); break;
        }
    }
    public void AddOverride(KeyCode keyCode, InputOverride inputOverride)
    {
        if (!overrides.ContainsKey(keyCode)) overrides[keyCode] = new();
        overrides[keyCode].Add(inputOverride);
        overrides[keyCode].Sort((a, b) => b.priority.CompareTo(a.priority));
    }
    public void RemoveOverride(int button, InputOverride inputOverride)
    {
        switch (button)
        {
            case 0: leftClickOverrides.Remove(inputOverride); break;
            case 1: rightClickOverrides.Remove(inputOverride); break;
        }
    }
    public void RemoveOverride(KeyCode keyCode, InputOverride inputOverride)
    {
        if (!overrides.ContainsKey(keyCode)) return;
        overrides[keyCode].Remove(inputOverride);
        if (overrides[keyCode].Count <= 0) overrides.Remove(keyCode);
    }
}
public class InputOverride
{
    public int priority;
    public Action onTrigger;
}
