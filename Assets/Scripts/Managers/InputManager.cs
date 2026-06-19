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
    // Keys are lowercase; lookups are normalized to lowercase so input is case-insensitive.
    Dictionary<string, KeyCode> stringToKeyCode = new Dictionary<string, KeyCode>()
    {
      {"a", KeyCode.A},
      {"b", KeyCode.B},
      {"c", KeyCode.C},
      {"d", KeyCode.D},
      {"e", KeyCode.E},
      {"f", KeyCode.F},
      {"g", KeyCode.G},
      {"h", KeyCode.H},
      {"i", KeyCode.I},
      {"j", KeyCode.J},
      {"k", KeyCode.K},
      {"l", KeyCode.L},
      {"m", KeyCode.M},
      {"n", KeyCode.N},
      {"o", KeyCode.O},
      {"p", KeyCode.P},
      {"q", KeyCode.Q},
      {"r", KeyCode.R},
      {"s", KeyCode.S},
      {"t", KeyCode.T},
      {"u", KeyCode.U},
      {"v", KeyCode.V},
      {"w", KeyCode.W},
      {"x", KeyCode.X},
      {"y", KeyCode.Y},
      {"z", KeyCode.Z},

      {"space", KeyCode.Space},
      {"shift", KeyCode.LeftShift},

      {"1", KeyCode.Alpha1},
      {"2", KeyCode.Alpha2},
      {"3", KeyCode.Alpha3},
      {"4", KeyCode.Alpha4},
      {"5", KeyCode.Alpha5},
      {"6", KeyCode.Alpha6},
      {"7", KeyCode.Alpha7},
      {"8", KeyCode.Alpha8},
      {"9", KeyCode.Alpha9},
      {"0", KeyCode.Alpha0},
    };
    Dictionary<KeyCode, string> keycodeToString;
    private void Awake()
    {
        keycodeToString = new Dictionary<KeyCode, string>();
        foreach (var pair in stringToKeyCode) keycodeToString[pair.Value] = pair.Key;
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
    public string KeycodeToString(KeyCode keyCode)
    {
        if (keycodeToString.ContainsKey(keyCode)) return keycodeToString[keyCode];
        return "";
    }
    public bool StringToKeyCode(string str, out KeyCode keyCode)
    {
        keyCode = KeyCode.None;
        if (str == null) return false;
        if (stringToKeyCode.TryGetValue(str.Trim().ToLower(), out keyCode)) return true;
        return false;
    }
}
public class InputOverride
{
    public int priority;
    public Action onTrigger;
}
