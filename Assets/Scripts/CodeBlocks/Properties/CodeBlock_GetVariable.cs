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
    Wildcard GetVariable(ulong hash)
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
                return target.variables[name];
            }
        }
        return new();
    }
    public override float GetNumber(ulong hash) => GetVariable(hash).number;
    public override bool GetCondition(ulong hash) => GetVariable(hash).condition;
    public override string GetString(ulong hash) => GetVariable(hash).str;
    public override MyGameObject GetObject(ulong hash) => GetVariable(hash).obj;
    public override MyAsset GetAsset(ulong hash) => GetVariable(hash).asset;
    public override Vector2 GetVector2(ulong hash) => GetVariable(hash).vector2;
    public override Color GetColor(ulong hash) => GetVariable(hash).color;
    public override ulong GetHash(ulong hash) => GetVariable(hash).hash;
    public override List<Wildcard> GetArray(ulong hash) => GetVariable(hash).array;
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return targetObject;
        yield return variableName;
    }
    public override float GetWidth() => rectTransform.rect.width;
}