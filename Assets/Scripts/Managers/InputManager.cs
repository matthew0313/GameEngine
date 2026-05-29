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
    Dictionary<char, KeyCode> charToKeycode = new Dictionary<char, KeyCode>()
    {
      {'a', KeyCode.A},
      {'b', KeyCode.B},
      {'c', KeyCode.C},
      {'d', KeyCode.D},
      {'e', KeyCode.E},
      {'f', KeyCode.F},
      {'g', KeyCode.G},
      {'h', KeyCode.H},
      {'i', KeyCode.I},
      {'j', KeyCode.J},
      {'k', KeyCode.K},
      {'l', KeyCode.L},
      {'m', KeyCode.M},
      {'n', KeyCode.N},
      {'o', KeyCode.O},
      {'p', KeyCode.P},
      {'q', KeyCode.Q},
      {'r', KeyCode.R},
      {'s', KeyCode.S},
      {'t', KeyCode.T},
      {'u', KeyCode.U},
      {'v', KeyCode.V},
      {'w', KeyCode.W},
      {'x', KeyCode.X},
      {'y', KeyCode.Y},
      {'z', KeyCode.Z},

      {'1', KeyCode.Alpha1},
      {'2', KeyCode.Alpha2},
      {'3', KeyCode.Alpha3},
      {'4', KeyCode.Alpha4},
      {'5', KeyCode.Alpha5},
      {'6', KeyCode.Alpha6},
      {'7', KeyCode.Alpha7},
      {'8', KeyCode.Alpha8},
      {'9', KeyCode.Alpha9},
      {'0', KeyCode.Alpha0},
    };
    Dictionary<KeyCode, char> keycodeToChar;
    private void Awake()
    {
        keycodeToChar = new Dictionary<KeyCode, char>();
        foreach (var pair in charToKeycode) keycodeToChar[pair.Value] = pair.Key;
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
        if (keycodeToChar.ContainsKey(keyCode)) return keycodeToChar[keyCode].ToString();
        return "";
    }
    public bool CharToKeycode(char c, out KeyCode keyCode)
    {
        keyCode = KeyCode.None;
        if (charToKeycode.ContainsKey(c))
        {
            keyCode = charToKeycode[c];
            return true;
        }
        return false;
    }
}
public class InputOverride
{
    public int priority;
    public Action onTrigger;
}
