using System;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentUtilities
{
    public static bool TryGetComponentInParents<T>(this Component component, out T result)
    {
        var current = component.transform;
        while (current != null)
        {
            if (current.TryGetComponent(out result)) return true;
            current = current.parent;
        }
        result = default;
        return false;
    }
    public static bool TryGetComponentInParents<T>(this GameObject gameObject, out T result)
    {
        var current = gameObject.transform;
        while (current != null)
        {
            if (current.TryGetComponent(out result)) return true;
            current = current.parent;
        }
        result = default;
        return false;
    }
}