using Cysharp.Threading.Tasks;
using Mono.Cecil;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Codeblock_InvokeFunction : ExecutableCodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;

    [SerializeField] RectTransform rectTransform;

    [SerializeField] Transform argumentElementAnchor;
    [SerializeField] SnapPoint_InvokeFunctionArg argumentElementPrefab;
    readonly List<SnapPoint_InvokeFunctionArg> argumentElements = new();

    [SerializeField] ObjectSnapPoint targetObject;
    [SerializeField] TMP_InputField functionNameField;
    [field:SerializeField] public ExecutableSnapPoint onFinish { get; private set; }
    int arguments = 0;

    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        targetObject.SetObject(owner as MyGameObject);
    }
    public void RemoveArgument(int index)
    {
        if (arguments <= 0) return;
        var tmp = argumentElements[index];
        argumentElements.RemoveAt(index);
        argumentElements.Add(tmp);
        tmp.Clear();
        arguments--;
        argumentElements[index].gameObject.SetActive(false);
    }
    public void AddArgument()
    {
        if (argumentElements.Count <= arguments)
        {
            var tmp = Instantiate(argumentElementPrefab, argumentElementAnchor);
            tmp.BindTarget(this);
            argumentElements.Add(tmp);
        }
        argumentElements[arguments].index = arguments;
        argumentElements[arguments].gameObject.SetActive(true);
        arguments++;
    }
    public override async UniTask<ExecutionFinishedInfo> Execute(ulong hash)
    {
        MyGameObject target = targetObject.GetObject(hash);
        if (target == null) return await onFinish.Execute(hash);
        var func = target.functions.Find(item => item.functionName == functionNameField.text);
        Debug.Log(target.functions[0].functionName);
        if (func != null)
        {
            Dictionary<string, Wildcard> arguments = new();
            for (int i = 0; i < func.parameters.Count; i++)
            {
                arguments.Add(func.parameters[i], argumentElements.Count >= i ? argumentElements[i].GetWildcard(hash) : new());
            }
            func.Execute(arguments);
        }
        else EditorSceneManager.Instance.AddLog(new()
        {
            type = MyLogType.Warning,
            message = $"No function named {functionNameField.text} in object {target.name}"
        });
        return await onFinish.Execute(hash);
    }

    public override float GetHeight()
    {
        return rectTransform.rect.height + onFinish.GetHeight();
    }
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Add Argument",
            ctx =>
            {
                AddArgument();
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return targetObject;
    }
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        ArgumentsWrapper wrapper = new() { arguments = new() };
        foreach (var i in argumentElements) wrapper.arguments.Add(i.Save());
        save.data.strings["arguments"] = JsonUtility.ToJson(wrapper);
        save.data.strings["functionName"] = functionNameField.text;
        return save;
    }
    ArgumentsWrapper loadWrapper;
    public override void EarlyLoad(CodeBlockSave save, bool resetUID = false)
    {
        base.EarlyLoad(save, resetUID);
        loadWrapper = JsonUtility.FromJson<ArgumentsWrapper>(save.data.strings["arguments"]);
        int i = 0;
        for (; i < loadWrapper.arguments.Count; i++)
        {
            if (argumentElements.Count <= i)
            {
                var tmp = Instantiate(argumentElementPrefab, argumentElementAnchor);
                tmp.BindTarget(this);
                argumentElements.Add(tmp);
            }
            argumentElements[i].index = i;
            argumentElements[i].EarlyLoad(loadWrapper.arguments[i], resetUID);
            argumentElements[i].gameObject.SetActive(true);
        }
        for (; i < argumentElements.Count; i++) argumentElements[i].gameObject.SetActive(false);
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        arguments = loadWrapper.arguments.Count;
        for (int i = 0; i < loadWrapper.arguments.Count; i++)
        {
            argumentElements[i].Load(loadWrapper.arguments[i]);
        }
        functionNameField.text = save.data.strings["functionName"];
    }
    [System.Serializable]
    struct ArgumentsWrapper
    {
        public List<SnapPointSave> arguments;
    }
}