using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<InputManager>("InputManager"));
        DontDestroyOnLoad(Instance);
    }
    readonly Dictionary<KeyCode, List<InputOverride>> overrides = new();
    public static bool GetKeyDown(KeyCode keyCode) => Input.GetKeyDown(keyCode) && !Instance.overrides.ContainsKey(keyCode);
    private void Update()
    {
        foreach (var keyCode in overrides.Keys)
        {
            if(Input.GetKeyDown(keyCode) && overrides[keyCode].Count > 0) overrides[keyCode][0].onTrigger.Invoke();
        }
    }
    public void AddOverride(KeyCode keyCode, InputOverride inputOverride)
    {
        if (!overrides.ContainsKey(keyCode)) overrides[keyCode] = new();
        overrides[keyCode].Add(inputOverride);
        overrides[keyCode].Sort((a, b) => b.priority.CompareTo(a.priority));
    }
    public void RemoveOverride(KeyCode keyCode, InputOverride inputOverride)
    {
        if (!overrides.ContainsKey(keyCode)) return;
        overrides[keyCode].Remove(inputOverride);
    }
}
public class InputOverride
{
    public int priority;
    public Action onTrigger;
}
