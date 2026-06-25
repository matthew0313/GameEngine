using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Codeblock_GetVariable : PropertyCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;
    public override PropertyType propertyType => PropertyType.Wildcard;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] ObjectSnapPoint targetObject;
    [SerializeField] StringSnapPoint variableName;
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        targetObject.SetObject(owner as MyGameObject);
    }
    public override float GetNumber(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetVariable"
            });
        }
        else
        {
            string name = variableName.GetValue(hash);
            if (target.variables.ContainsKey(name))
            {
                return target.variables[name].number;
            }
        }
        return base.GetNumber(hash);
    }
    public override bool GetCondition(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetVariable"
            });
        }
        else
        {
            string name = variableName.GetValue(hash);
            if (target.variables.ContainsKey(name))
            {
                return target.variables[name].condition;
            }
        }
        return base.GetCondition(hash);
    }
    public override string GetString(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetVariable"
            });
        }
        else
        {
            string name = variableName.GetValue(hash);
            if (target.variables.ContainsKey(name))
            {
                return target.variables[name].str;
            }
        }
        return base.GetString(hash);
    }
    public override MyGameObject GetObject(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetVariable"
            });
        }
        else
        {
            string name = variableName.GetValue(hash);
            if (target.variables.ContainsKey(name))
            {
                return target.variables[name].obj;
            }
        }
        return base.GetObject(hash);
    }
    public override MyAsset GetAsset(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetVariable"
            });
        }
        else
        {
            string name = variableName.GetValue(hash);
            if (target.variables.ContainsKey(name))
            {
                return target.variables[name].asset;
            }
        }
        return base.GetAsset(hash);
    }
    public override Vector2 GetVector2(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetVariable"
            });
        }
        else
        {
            string name = variableName.GetValue(hash);
            if (target.variables.ContainsKey(name))
            {
                return target.variables[name].vector2;
            }
        }
        return base.GetVector2(hash);
    }
    public override Color GetColor(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetVariable"
            });
        }
        else
        {
            string name = variableName.GetValue(hash);
            if (target.variables.ContainsKey(name))
            {
                return target.variables[name].color;
            }
        }
        return base.GetColor(hash);
    }
    public override ulong GetHash(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Error,
                message = $"No target for GetVariable"
            });
        }
        else
        {
            string name = variableName.GetValue(hash);
            if (target.variables.ContainsKey(name))
            {
                return target.variables[name].hash;
            }
        }
        return base.GetHash(hash);
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return targetObject;
        yield return variableName;
    }
    public override float GetWidth() => rectTransform.rect.width;
}