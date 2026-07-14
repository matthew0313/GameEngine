using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Codeblock_GetArrayElem : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Wildcard;
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] ArraySnapPoint targetArray;
    [SerializeField] NumericSnapPoint index;
    public override float GetWidth() => rectTransform.rect.width;
    Wildcard GetArrayElem(ulong hash)
    {
        List<Wildcard> array = targetArray.GetArray(hash);
        if (array == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetArrayElem"
            });
        }
        else
        {
            int idx = index.GetIntNumber(hash);
            if (idx < 0 || idx >= array.Count)
            {
                EditorSceneManager.Instance.AddLog(new()
                {
                    type = MyLogType.Error,
                    message = $"Index {idx} out of bounds for array of size {array.Count}"
                });
            }
            else
            {
                return array[idx];
            }
        }
        return new();
    }
    public override float GetNumber(ulong hash) => GetArrayElem(hash).number;

    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return targetArray;
        yield return index;
    }
}