using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class DataUnit
{
    public SerializableDictionary<string, int> integers = new();
    public SerializableDictionary<string, float> floats = new();
    public SerializableDictionary<string, string> strings = new();
    public SerializableDictionary<string, bool> bools = new();
    public void SaveVector2(string key, Vector2 value)
    {
        floats[key + ".x"] = value.x;
        floats[key + ".y"] = value.y;
    }
    public bool TryLoadVector2(string key, out Vector2 value)
    {
        value = new();
        if (floats.TryGetValue(key + ".x", out float x) && floats.TryGetValue(key + ".y", out float y))
        {
            value = new Vector2(x, y);
            return true;
        }
        return false;
    }
    public Vector2 LoadVector2(string key)
    {
        return new Vector2(floats[key + ".x"], floats[key + ".y"]);
    }
}